from app.main import db

class Field(db.Model):
    """Field domain"""
    __tablename__ = "Field"

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    created_at = db.Column(db.DateTime, nullable=False, default=db.func.now())
    updated_at = db.Column(db.DateTime, nullable=False, default=db.func.now(), onupdate=db.func.now())

    name = db.Column(db.String(255), nullable=False)
    unit_id = db.Column(db.Integer) #this property needs to be handled by orchestrator, so no foreign key
    device_type_id = db.Column(db.Integer) #this property needs to be handled by orchestrator, so no foreign key
    loggable = db.Column(db.Boolean, nullable=False, default=False)

    log_values = db.relationship('LogValue', back_populates='field') #is needed for relationship

    def __repr__(self):
        return "<Field '{}'>".format(self.name)
