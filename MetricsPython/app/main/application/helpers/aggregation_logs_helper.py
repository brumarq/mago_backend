from typing import List
from io import StringIO
import csv
from flask import abort

class AggregationLogsHelper:

    @staticmethod
    def generate_csv(aggregated_logs):

        if (aggregated_logs is None):
            raise abort(404, "There were no data provided for the CSV or the data is empty")

        headers = [
            'ID',
            'Created At',
            'Updated At',
            'Average Value',
            'Min Value',
            'Max Value',
            'Device ID',
            'Field ID',
            'Field Created At',
            'Field Updated At',
            'Field Name',
            'Field Unit ID',
            'Field Device Type ID',
            'Field Loggable',
        ]

        csv_data = [headers]

        for log in aggregated_logs:
            field = log.field
            csv_data.append([
                log.id,
                log.created_at,
                log.updated_at,
                log.average_value,
                log.min_value,
                log.max_value,
                log.device_id,
                field.id,
                field.created_at,
                field.updated_at,
                field.name,
                field.unit_id,
                field.device_type_id,
                field.loggable,
            ])

        # Create a response object with CSV MIME type
        csv_file = StringIO()
        csv_writer = csv.writer(csv_file)
        csv_writer.writerows(csv_data)

        return csv_file