import pytest
from app.main.domain.entities.field import Field

@pytest.mark.parametrize("repository", [Field], indirect=True)
def test_create(repository):
    entity_to_create = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)

    # Call the create method
    created_entity = repository.create(entity_to_create)

    # Assert that the entity is created and returned correctly
    assert created_entity.id is not None
    assert created_entity.id == 1
    assert created_entity.name == 'TestName'
    assert created_entity.unit_id == 1
    assert created_entity.device_type_id == 1
    assert created_entity.loggable is True

@pytest.mark.parametrize("repository", [Field], indirect=True)
def test_get_all(repository):

    #Insert random field into db
    entity_to_create = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)
    repository.create(entity_to_create)

    # Call the get_all method
    all_entities = repository.get_all()

    # Assert that the result is a list and not empty
    assert isinstance(all_entities, list)
    assert len(all_entities) == 1


@pytest.mark.parametrize("repository", [Field], indirect=True)
def test_get_by_condition(repository):

    # Insert random field into db
    entity_to_create = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)
    repository.create(entity_to_create)

    # Call the get_by_condition method with a condition
    condition = (repository.model.name == 'TestName') & (repository.model.id == 1)
    entity = repository.get_by_condition(condition)

    # Assert that the entity is retrieved correctly
    assert entity is not None
    assert entity.name == 'TestName'
    assert entity.id == 1


@pytest.mark.parametrize("repository", [Field], indirect=True)
def test_get_all_by_condition(repository):

    #Create two entities
    entity_to_create1 = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)
    repository.create(entity_to_create1)

    entity_to_create2 = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)
    repository.create(entity_to_create2)

    # Call the get_all_by_condition method with a condition
    condition = repository.model.name == 'TestName'
    entities = repository.get_all_by_condition(condition)

    # Assert that its a list and that there are indeed 2 entries based on the name
    assert isinstance(entities, list)
    assert len(entities) == 2


@pytest.mark.parametrize("repository", [Field], indirect=True)
def test_delete(repository):

    #Create an entity
    entity_to_create = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)
    created_entity = repository.create(entity_to_create)

    # Call the delete method
    result = repository.delete(created_entity.id)

    # Assert that the entity is deleted
    assert result is True

    # Make sure it is not in the database anymore
    assert not repository.get_by_condition(Field.id == 1)


@pytest.mark.parametrize("repository", [Field], indirect=True)
def test_update(repository):
    # Create an entity
    entity_to_create = repository.model(name='TestName', unit_id=1, device_type_id=1, loggable=True)
    created_entity = repository.create(entity_to_create)
    print(f"BEFORE: {created_entity.name}")
    # Update the entity's name
    created_entity.name = 'UpdatedName'

    # Call the update method
    updated_entity = repository.update(created_entity)

    # Assert that the update was successful
    assert updated_entity is True

    # Check if the name is actually updated in db
    condition = (repository.model.name == 'UpdatedName') & (repository.model.id == 1)
    assert repository.get_by_condition(condition)

    
