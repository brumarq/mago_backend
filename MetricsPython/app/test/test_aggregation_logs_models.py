from app.main.domain.entities.field import Field
from app.main.domain.entities.weekly_average import WeeklyAverage
from app.main.domain.entities.monthly_average import MonthlyAverage
from app.main.domain.entities.yearly_average import YearlyAverage
from app.test.conftest import db
import pytest

@pytest.mark.parametrize("average_value, min_value, max_value, device_id, field_id", [
    (10.5, 8.0, 12.0, 1, 1),
    (20.3, 15.0, 25.0, 2, 1),
    (30.0, 25.0, 35.0, 3, 2),
])
def test_weekly_average_creation(app, average_value, min_value, max_value, device_id, field_id):
    field = Field(name="TestField", unit_id=1, device_type_id=1, loggable=True)

    weekly_average = WeeklyAverage(
        average_value=average_value,
        min_value=min_value,
        max_value=max_value,
        device_id=device_id,
        field_id=field_id
    )

    assert weekly_average.average_value == average_value
    assert weekly_average.min_value == min_value
    assert weekly_average.max_value == max_value
    assert weekly_average.device_id == device_id
    assert weekly_average.field_id == field_id

    with app.app_context():
        db.session.add(field)
        db.session.add(weekly_average)
        db.session.commit()

        assert weekly_average.created_at is not None
        assert weekly_average.updated_at is not None
        assert weekly_average.created_at <= weekly_average.updated_at

        assert WeeklyAverage.query.filter_by()
        assert WeeklyAverage.query.filter_by(average_value=average_value).first() == weekly_average


@pytest.mark.parametrize("average_value, min_value, max_value, device_id, field_id", [
    (100.5, 80.0, 120.0, 1, 1),
    (200.3, 150.0, 250.0, 2, 1),
    (300.0, 250.0, 350.0, 3, 2),
])
def test_monthly_average_creation(client, app, average_value, min_value, max_value, device_id, field_id):
    field = Field(name="TestField", unit_id=1, device_type_id=1, loggable=True)

    monthly_average = MonthlyAverage(
        average_value=average_value,
        min_value=min_value,
        max_value=max_value,
        device_id=device_id,
        field_id=field_id
    )

    assert monthly_average.average_value == average_value
    assert monthly_average.min_value == min_value
    assert monthly_average.max_value == max_value
    assert monthly_average.device_id == device_id
    assert monthly_average.field_id == field_id

    with app.app_context():
        db.session.add(field)
        db.session.add(monthly_average)
        db.session.commit()

        assert monthly_average.created_at is not None
        assert monthly_average.updated_at is not None
        assert monthly_average.created_at <= monthly_average.updated_at

        assert MonthlyAverage.query.filter_by(average_value=average_value).first() == monthly_average
        filtered_result = MonthlyAverage.query.filter_by(id=monthly_average.id).first()
        assert filtered_result.id != 2

@pytest.mark.parametrize("average_value, min_value, max_value, device_id, field_id", [
    (500.5, 400.0, 600.0, 1, 1),
    (600.3, 500.0, 700.0, 2, 1),
    (700.0, 600.0, 800.0, 3, 2),
])
def test_yearly_average_creation_and_filtering(app, average_value, min_value, max_value, device_id, field_id):
    field = Field(name="TestField", unit_id=1, device_type_id=1, loggable=True)

    yearly_average = YearlyAverage(
        average_value=average_value,
        min_value=min_value,
        max_value=max_value,
        device_id=device_id,
        field_id=field_id
    )

    assert yearly_average.average_value == average_value
    assert yearly_average.min_value == min_value
    assert yearly_average.max_value == max_value
    assert yearly_average.device_id == device_id
    assert yearly_average.field_id == field_id

    with app.app_context():
        db.session.add(field)
        db.session.add(yearly_average)
        db.session.commit()

        # Check dates after committing to the database
        assert yearly_average.created_at is not None
        assert yearly_average.updated_at is not None
        assert yearly_average.created_at <= yearly_average.updated_at

        assert YearlyAverage.query.filter_by(average_value=average_value).first() == yearly_average

        filtered_result = YearlyAverage.query.filter_by(device_id=device_id, field_id=field_id).first()
        assert filtered_result is not None
        assert filtered_result.id == 1