from app.main import db
from app.main.domain.entities.field import Field


class YearlyAverage(db.Model):
    """Yearly aggregated log averages"""
    __tablename__ = 'YearlyAverage'

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    created_at = db.Column(db.DateTime, nullable=False, default=db.func.now())
    updated_at = db.Column(db.DateTime, nullable=False, default=db.func.now(), onupdate=db.func.now())

    average_value = db.Column(db.Float, nullable=False)

    device_id = db.Column(db.Integer, nullable=False) #this property needs to be handled by orchestrator, so no foreign key

    field_id = db.Column(db.Integer, db.ForeignKey(Field.id))
    field = db.relationship('Field', back_populates='log_values') #needed for relationship
