from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from app.main.infrastructure.repositories.repository import Repository  # Adjust the import path as needed
from sqlalchemy import func, and_
from app.main import db
from sqlalchemy.exc import SQLAlchemyError

class MetricsRepository(Repository):

    def get_latest_log_values_by_device_id(self, device_id, page_number=None, page_size=None):
        try:
            latest_timestamp_subquery = (
                db.session.query(
                    LogValue.field_id,
                    func.max(LogValue.created_at).label('latest_timestamp')
                )
                .join(LogCollection)
                .filter(LogCollection.device_id == device_id)
                .group_by(LogValue.field_id)
                .subquery()
            )

            # Query the LogValue entries using the latest timestamps and device_id
            query = self.model.query.join(
                latest_timestamp_subquery,
                and_(
                    LogValue.field_id == latest_timestamp_subquery.c.field_id,
                    LogValue.created_at == latest_timestamp_subquery.c.latest_timestamp,
                ),
            ).join(LogCollection).filter(LogCollection.device_id == device_id)

            # Set a default values for page_number and page_seize
            page_number = page_number or 1
            page_size = page_size or 50

            query = query.order_by(self.model.created_at).offset((page_number - 1) * page_size).limit(page_size)

            latest_log_values = query.all()

            return latest_log_values
        
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e