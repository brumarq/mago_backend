from app.main import db

class LogCollectionType(db.Model):
    """LogCollectionType domain"""
    __tablename__ = 'LogCollectionType'

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    created_at = db.Column(db.DateTime, nullable=False, default=db.func.now())
    updated_at = db.Column(db.DateTime, nullable=False, default=db.func.now(), onupdate=db.func.now())

    log_collections = db.relationship('LogCollection', back_populates='log_collection_type', lazy=True) #is needed for relationship

    def __repr__(self):
        return "<LogCollectionType '{}'>".format(self.collections)