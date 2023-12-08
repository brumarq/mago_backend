# import unittest

# import datetime

# from app.main import db
# from app.main.domain.entities.aggregated_log import AggregatedLog
# from app.main.domain.entities.field import Field
# from app.test.test_base import BaseTest

# class TestAggregatedLogsModel(BaseTest):
#     def test_field_in_aggregated_logs(self):

#         field = Field(
#             name="temp",
#             unit_id=1,
#             device_type_id=1,
#             loggable=True
#         )

#         db.session.add(field)
#         db.session.commit()

#         aggregated_log = AggregatedLog(
#             field_id = field.id,
#             type = "sick type",
#             average_value = 10.5,
#             min_value = 8.5,
#             max_value = 11.5
#         )
#         db.session.add(aggregated_log)
#         db.session.commit()

#         saved_aggregated_log = AggregatedLog.query.filter_by(field_id=aggregated_log.field_id).first()

#         self.assertIsNotNone(saved_aggregated_log.created_at)
#         self.assertIsNotNone(saved_aggregated_log.updated_at)

#         self.assertIsInstance(saved_aggregated_log.created_at, datetime.datetime)
#         self.assertIsInstance(saved_aggregated_log.updated_at, datetime.datetime)

#         self.assertEqual(saved_aggregated_log.field_id, aggregated_log.field_id)
    

# if __name__ == '__main__':
#     unittest.main()