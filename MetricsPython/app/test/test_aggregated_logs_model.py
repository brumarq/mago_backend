import unittest

import datetime
import time

from app.main import db
from app.main.domain.entities.aggregated_log import AggregatedLog
from app.main.domain.entities.field import Field
from app.test.test_base import BaseTest

class TestAggregatedLogsModel(BaseTest):
    def test_creation_of_agg_log_should_make_proper_dates_and_have_correct_field(self):

        field = Field(
            name="temp",
            unit_id=1,
            device_type_id=1,
            loggable=True
        )

        db.session.add(field)
        db.session.commit()

        aggregated_log = AggregatedLog(
            field_id = field.id,
            type = "sick type",
            average_value = 10.5,
            min_value = 8.5,
            max_value = 11.5
        )
        db.session.add(aggregated_log)
        db.session.commit()

        saved_aggregated_log = AggregatedLog.query.filter_by(field_id=aggregated_log.field_id).first()

        self.assertIsNotNone(saved_aggregated_log.created_at)
        self.assertIsNotNone(saved_aggregated_log.updated_at)

        self.assertIsInstance(saved_aggregated_log.created_at, datetime.datetime)
        self.assertIsInstance(saved_aggregated_log.updated_at, datetime.datetime)

        self.assertEqual(saved_aggregated_log.field_id, aggregated_log.field_id)

    def test_updating_agg_log_should_change_updated_at(self):
        # Create new field
        field = Field(
             name="temp",
             unit_id=1,
             device_type_id=1,
             loggable=True
         )

        #Store in db
        db.session.add(field)
        db.session.commit()
        
        # Create a new aggregated log
        new_aggregated_log = AggregatedLog(
            field_id=field.id,
            type='some_type', 
            average_value=10.0, 
            min_value=5.0, 
            max_value=15.0
        )

        # Store in db
        db.session.add(new_aggregated_log)
        db.session.commit()

        # Sleep for a bit to see the difference
        time.sleep(1)

        #Keep track of updated_at value
        original_updated_at = new_aggregated_log.updated_at

        # Update the aggregated log
        new_aggregated_log.type = 'updated_type'
        db.session.commit()

        new_updated_at = new_aggregated_log.updated_at

        #Compare (they should not be equal after updatation)
        self.assertNotEqual(original_updated_at, new_updated_at)

    

if __name__ == '__main__':
    unittest.main()