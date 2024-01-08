from app.main.domain.entities.log_value import LogValue
from app.main.domain.entities.log_collection import LogCollection
from app.main.infrastructure.repositories.repository import Repository  # Adjust the import path as needed
from sqlalchemy import func, and_
from app.main import db
from sqlalchemy.exc import SQLAlchemyError

class MetricsRepository(Repository):

    def get_latest_log_values_by_device_id(self, device_id):
        try:
            latest_timestamp_subquery = ( # Get latest timestamps (created_at) for each Field in a Device
                db.session.query(
                    LogValue.field_id,
                    func.max(LogValue.created_at).label('latest_timestamp')
                )
                .group_by(LogValue.field_id)
                .subquery()
            )

            return self.model.query.join(latest_timestamp_subquery, and_( # Return all unique entries with latest timestamps 
                LogValue.field_id == latest_timestamp_subquery.c.field_id,
                LogValue.created_at == latest_timestamp_subquery.c.latest_timestamp
            )).join(LogCollection).filter(LogCollection.device_id == device_id).all()
        except SQLAlchemyError as e:
            db.session.rollback()
            raise e