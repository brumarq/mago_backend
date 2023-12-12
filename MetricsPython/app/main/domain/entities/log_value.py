from app.main import db
from app.main.domain.entities.field import Field
from app.main.domain.entities.log_collection import LogCollection

class LogValue(db.Model):
    """LogValue domain"""
    __tablename__ = 'LogValue'

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    created_at = db.Column(db.DateTime, nullable=False, default=db.func.now())
    updated_at = db.Column(db.DateTime, nullable=False, default=db.func.now(), onupdate=db.func.now())

    value = db.Column(db.Float, nullable=False)

    field_id = db.Column(db.Integer, db.ForeignKey(Field.id))
    field = db.relationship('Field', back_populates='log_values') #needed for relationship

    log_collection_id = db.Column(db.Integer, db.ForeignKey(LogCollection.id))
    log_collection = db.relationship('LogCollection', back_populates='log_values') #needed for relatipnship
    
    def __repr__(self):
        return "<LogValue '{}'>".format(self.value)