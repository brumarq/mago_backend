from app.main import db
from app.main.domain.entities.log_collection_type import LogCollectionType

class LogCollection(db.Model):
    """LogCollection domain"""
    __tablename__ = 'LogCollection'

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    created_at = db.Column(db.DateTime, nullable=False, default=db.func.now())
    updated_at = db.Column(db.DateTime, nullable=False, default=db.func.now(), onupdate=db.func.now())

    device_id = db.Column(db.Integer, nullable=False) #this property needs to be handled by orchestrator, so no foreign key

    log_values = db.relationship('LogValue', back_populates='log_collection')

    log_collection_type_id = db.Column(db.Integer, db.ForeignKey(LogCollectionType.id))
    log_collection_type = db.relationship('LogCollectionType', back_populates='log_collections', lazy=True)

    def __repr__(self):
        return "<LogCollection '{}'>".format(self.values)