# from typing import List
# from io import StringIO
# import csv
# from app.main.domain.entities.aggregated_log import AggregatedLog
# from flask import abort

# class MetricsHelper:

#     @staticmethod
#     def generate_csv(aggregated_logs: List[AggregatedLog]):

#         if (aggregated_logs is None or len(aggregated_logs) == 0):
#             abort(404, "There were no data provided for the CSV or the data is empty")

#         headers = ['ID', 'Created At', 'Updated At', 'Field ID', 'Field Name', 'Log Type', 'Average Value', 'Min Value', 'Max Value', 'Unit ID', 'Device Type ID', 'Loggable']

#         csv_data = [headers]

#         for log in aggregated_logs:
#             field = log.field
#             csv_data.append([
#                 log.id,
#                 log.created_at,
#                 log.updated_at,
#                 field.id,
#                 field.name,
#                 log.type,
#                 log.average_value,
#                 log.min_value,
#                 log.max_value,
#                 field.unit_id,
#                 field.device_type_id,
#                 field.loggable
#             ])

#         # Create a response object with CSV MIME type
#         csv_file = StringIO()
#         csv_writer = csv.writer(csv_file)
#         csv_writer.writerows(csv_data)

#         return csv_file