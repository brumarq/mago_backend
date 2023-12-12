from app.main import db
from app.main.domain.entities.field import Field


class AggregatedLog(db.Model):
    """Aggregated log"""
    __tablename__ = 'AggregatedLog'

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    created_at = db.Column(db.DateTime, nullable=False, default=db.func.now())
    updated_at = db.Column(db.DateTime, nullable=False, default=db.func.now(), onupdate=db.func.now())
    
    field_id = db.Column(db.Integer, db.ForeignKey(Field.id), nullable=False)
    field = db.relationship('Field', back_populates='aggregated_logs')

    type = db.Column(db.String(255), nullable=False) 

    average_value = db.Column(db.Float, nullable=False)
    min_value = db.Column(db.Float, nullable=False)
    max_value = db.Column(db.Float, nullable=False)

    def __repr__(self):
        return "<AggregatedLog '{}'>".format(self.type)
