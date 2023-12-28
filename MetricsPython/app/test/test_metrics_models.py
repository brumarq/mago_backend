from app.main.domain.entities.field import Field
from app.main.domain.entities.log_collection import LogCollection
from app.main.domain.entities.log_collection_type import LogCollectionType
from app.main.domain.entities.log_value import LogValue
from app.test.conftest import db
import pytest

@pytest.mark.parametrize("name, unit_id, device_type_id, loggable", [
    ("Temperature", 1, 1, True),
    ("Pressure", 2, 1, False),
    ("humidity", 3, 2, True),
    ("pOwEr", 3, 2, True),
])
def test_metrics_field_creation(app, name, unit_id, device_type_id, loggable):
    field = Field(
        name=name,
        unit_id=unit_id,
        device_type_id=device_type_id,
        loggable=loggable
    )
    assert field.name == name
    assert field.unit_id == unit_id
    assert field.device_type_id == device_type_id
    assert field.loggable == loggable

    with app.app_context():
        db.session.add(field)
        db.session.commit()

        assert field.created_at is not None
        assert field.updated_at is not None
        assert field.created_at <= field.updated_at

        assert Field.query.count() == 1
        assert Field.query.filter_by(name=name).first() == field

@pytest.mark.parametrize("device_id, log_collection_type_id", [
    (1, 1),
    (2, 2),
    (3, 3),
])
def test_log_collection_creation(app, device_id, log_collection_type_id):
    log_collection = LogCollection(
        device_id=device_id,
        log_collection_type_id=log_collection_type_id
    )
    assert log_collection.device_id == device_id
    assert log_collection.log_collection_type_id == log_collection_type_id

    with app.app_context():
        db.session.add(log_collection)
        db.session.commit()

        assert log_collection.created_at is not None
        assert log_collection.updated_at is not None
        assert log_collection.created_at <= log_collection.updated_at

        assert LogCollection.query.count() == 1
        assert LogCollection.query.filter_by(device_id=device_id).first() == log_collection


@pytest.mark.parametrize("log_collections", [
    [LogCollection(device_id=1), LogCollection(device_id=2)],
    [LogCollection(device_id=3), LogCollection(device_id=4)],
])
def test_log_collection_type_creation(app, log_collections):
    log_collection_type = LogCollectionType(log_collections=log_collections)
    
    with app.app_context():
        db.session.add(log_collection_type)
        db.session.commit()

        assert log_collection_type.created_at is not None
        assert log_collection_type.updated_at is not None
        assert log_collection_type.created_at <= log_collection_type.updated_at

        assert LogCollectionType.query.count() == 1
        assert LogCollectionType.query.filter_by(id=log_collection_type.id).first() == log_collection_type

@pytest.mark.parametrize("value, field_id, log_collection_id", [
    (10.5, 1, 1),
    (20.3, 2, 1),
    (30.0, 3, 2),
])
def test_log_value_creation(app, value, field_id, log_collection_id):
    # Creating associated Field and LogCollection objects for the test
    field = Field(name="TestField", unit_id=1, device_type_id=1, loggable=True)
    log_collection = LogCollection(device_id=1, log_collection_type_id=1)

    log_value = LogValue(
        value=value,
        field_id=field_id,
        log_collection_id=log_collection_id
    )

    assert log_value.value == value
    assert log_value.field_id == field_id
    assert log_value.log_collection_id == log_collection_id

    with app.app_context():
        db.session.add(field)
        db.session.add(log_collection)
        db.session.add(log_value)
        db.session.commit()

        assert log_value.created_at is not None
        assert log_value.updated_at is not None
        assert log_value.created_at <= log_value.updated_at

        assert LogValue.query.count() == 1
        assert LogValue.query.filter_by(value=value).first() == log_value
