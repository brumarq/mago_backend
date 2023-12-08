import unittest
from app.main import db
from app.main.domain.entities.log_collection_type import LogCollectionType
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.entities.field import Field
from app.main.domain.entities.log_value import LogValue
from app.test.test_base import BaseTest

class TestMetricsModel(BaseTest):
    def test_creation_of_metrics_should_provide_proper_ids(self):

        log_collection_type = LogCollectionType()

        db.session.add(log_collection_type)
        db.session.commit()

        log_collection = LogCollection(
            device_id = 1,
            log_collection_type_id=log_collection_type.id
        )

        db.session.add(log_collection)
        db.session.commit()

        field = Field(
             name="temp",
             unit_id=1,
             device_type_id=1,
             loggable=True
         )

        #Store in db
        db.session.add(field)
        db.session.commit()

        log_value = LogValue(
            value = 12.5,
            field_id = field.id,
            log_collection_id = log_collection.id
        )

        db.session.add(log_value)
        db.session.commit()

        # Test relationships
        self.assertEqual(log_collection.log_collection_type_id, log_collection_type.id)
        self.assertEqual(log_value.log_collection_id, log_collection.id)
        self.assertEqual(log_value.field_id, field.id)


if __name__ == '__main__':
    unittest.main()