using System.Linq.Expressions;
using Application.ApplicationServices;
using Application.Mappers.AutoMapper;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Application.Tests;

[TestFixture]
public class UnitServiceTests
{
    private IMapper _mapper;
    private Mock<IRepository<Unit>> _mockRepository;

    [SetUp]
    public void Setup()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new Profiles()); });
        _mapper = mappingConfig.CreateMapper();

        _mockRepository = new Mock<IRepository<Unit>>();
    }
    
    [Test]
    public async Task GetUnitByIdAsync_ExistingUnit_ReturnsUnitDTO()
    {
        // Arrange
        int unitId = 1;
        var expectedUnit = new Unit
        {
            Id = unitId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Test Unit",
            Symbol = "TU",
            Factor = 2f,
            Offset = 1f,
        };

        _mockRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Unit, bool>>>()))
            .ReturnsAsync(expectedUnit);

        var service = new UnitService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.GetUnitByIdAsync(unitId);

        // Assert
        Assert.IsNotNull(result);
    }
    
    [Test]
    public async Task GetUnitByIdAsync_NonExistingUnit_ReturnsNull()
    {
        // Arrange
        int nonExistingUnitId = 999; // An ID that does not exist

        _mockRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Unit, bool>>>()))
            .ReturnsAsync((Unit)null);

        var service = new UnitService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.GetUnitByIdAsync(nonExistingUnitId);

        // Assert
        Assert.IsNull(result);
    }
}