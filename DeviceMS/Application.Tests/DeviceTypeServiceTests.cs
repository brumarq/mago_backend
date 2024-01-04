using System.Linq.Expressions;
using Application.ApplicationServices;
using Application.DTOs.DeviceType;
using Application.Exceptions;
using Application.Mappers.AutoMapper;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Application.Tests;

[TestFixture]
public class DeviceTypeServiceTests
{
    private IMapper _mapper;
    private Mock<IRepository<DeviceType>> _mockRepository;

    [SetUp]
    public void Setup()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new Profiles()); });
        _mapper = mappingConfig.CreateMapper();

        _mockRepository = new Mock<IRepository<DeviceType>>();
    }

    #region Create DeviceType Tests

    [Test]
    public async Task CreateDeviceTypeAsync_Success_ReturnsDeviceTypeResponsDTO()
    {
        // Arrange
        var newDeviceTypeDto = new CreateDeviceTypeDTO
        {
            Name = "New Device Type"
        };

        // Expected DeviceType returned from repository
        var createdDeviceType = new DeviceType
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = newDeviceTypeDto.Name
        };

        // Setup DeviceType repository response
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<DeviceType>())).ReturnsAsync(createdDeviceType);

        var expectedResult = new DeviceTypeResponseDTO
        {
            Id = createdDeviceType.Id,
            CreatedAt = createdDeviceType.CreatedAt,
            UpdatedAt = createdDeviceType.UpdatedAt,
            Name = createdDeviceType.Name
        };

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.CreateDeviceTypeAsync(newDeviceTypeDto);

        Assert.IsNotNull(result);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedResult.Id));
            Assert.That(result.CreatedAt, Is.EqualTo(expectedResult.CreatedAt));
            Assert.That(result.UpdatedAt, Is.EqualTo(expectedResult.UpdatedAt));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
        });
    }

    public static IEnumerable<TestCaseData> InvalidCreateDeviceTypeTestData()
    {
        // Object is null
        yield return new TestCaseData(null, "The object cannot be null"
        );
        // Name is null
        yield return new TestCaseData(new CreateDeviceTypeDTO
            {
                Name = null
            }, "The name property is required to be filled out"
        );
        //Name is empty
        yield return new TestCaseData(new CreateDeviceTypeDTO
            {
                Name = ""
            }, "The name property is required to be filled out"
        );
    }

    [Test, TestCaseSource(nameof(InvalidCreateDeviceTypeTestData))]
    public void CreateDeviceTypeAsync_InvalidDate_ThrowsBadRequestException(CreateDeviceTypeDTO invalidDeviceTypeDto,
        string expectedMessage)
    {
        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        var ex = Assert.ThrowsAsync<BadRequestException>(async () =>
            await service.CreateDeviceTypeAsync(invalidDeviceTypeDto));
        
        Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
    }

    #endregion

    #region Retrieve List of Device Types Tests

    [Test]
    public async Task GetDeviceTypesAsync_ReturnsAllDeviceTypes()
    {
        // Arrange
        var deviceTypes = new List<DeviceType>
        {
            new DeviceType
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 1"
            },
            new DeviceType
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 2"
            },
            new DeviceType
            {
                Id = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 3"
            }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(deviceTypes);

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);
        
        // Act
        var result = await service.GetDeviceTypesAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(deviceTypes.Count()));
    }

    [Test]
    public async Task GetDeviceTypesAsync_NoDeviceTypes_ReturnsListOfDeviceTypes()
    {
        // Arrange
        var emptyDeviceTypeList = new List<DeviceType>();

        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyDeviceTypeList);
        
        var service = new DeviceTypeService(_mapper, _mockRepository.Object);
        
        // Act
        var result = await service.GetDeviceTypesAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetDeviceTypeDropdown_ReturnsLabelValueOptionsForAllDeviceTypes()
    {
        // Arrange
        var deviceTypes = new List<DeviceType>
        {
            new DeviceType
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 1"
            },
            new DeviceType
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 2"
            },
            new DeviceType
            {
                Id = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 3"
            }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(deviceTypes);

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.GetDeviceTypeDropdown();

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(deviceTypes.Count));
        
        foreach (var option in result)
        {
            var correspondingDeviceType = deviceTypes.FirstOrDefault(dt => dt.Id == option.Value);
            Assert.IsNotNull(correspondingDeviceType);
            Assert.That(option.Label, Is.EqualTo(correspondingDeviceType?.Name));
        }
    }
    
    #endregion

    #region Retrieve Specific DeviceType Tests

    [Test]
    public async Task GetDeviceTypeByIdAsync_ExistingDeviceType_ReturnsDeviceTypeResponseDTO()
    {
        // Arrange
        int deviceTypeId = 1;
        var expectedDeviceType = new DeviceType
        {
            Id = deviceTypeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Test Device Type"
        };

        _mockRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<DeviceType, bool>>>()))
            .ReturnsAsync(expectedDeviceType);

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.GetDeviceTypeByIdAsync(deviceTypeId);

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task GetDeviceTypeByIdAsync_NonExistingDeviceType_ReturnsNull()
    {
        // Arrange
        int nonExistingDeviceTypeId = -1; // An ID that does not exist

        _mockRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<DeviceType, bool>>>()))
            .ReturnsAsync((DeviceType)null);

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.GetDeviceTypeByIdAsync(nonExistingDeviceTypeId);

        // Assert
        Assert.IsNull(result);
    }
    
    #endregion

    #region Update DeviceType Tests

    [Test]
    public async Task UpdateDeviceTypeAsync_ExistingDeviceType_ReturnsTrue()
    {
        // Arrange
        var updateDeviceTypeDto = new UpdateDeviceTypeDTO
        {
            Name = "New Name"
        };
        int deviceTypeId = 1; // Assuming this is an existing device type ID

        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<DeviceType>()))
            .ReturnsAsync(true);

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.UpdateDeviceTypeAsync(deviceTypeId, updateDeviceTypeDto);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task UpdateDeviceTypeAsync_NonExistingDeviceType_ReturnsFalseOrNull()
    {
        // Arrange
        var updateDeviceTypeDto = new UpdateDeviceTypeDTO
        {
            Name = "New Name"
        };
        int nonExistingDeviceTypeId = -1; // An ID that does not exist

        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<DeviceType>()))
            .ReturnsAsync(false);

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.UpdateDeviceTypeAsync(nonExistingDeviceTypeId, updateDeviceTypeDto);

        // Assert
        Assert.IsFalse(result);
    }

    
    public static IEnumerable<TestCaseData> InvalidUpdateDeviceTypeTestData()
    {
        // Object is null
        yield return new TestCaseData(null, "The object cannot be null"
        );
        // Name is null
        yield return new TestCaseData(new UpdateDeviceTypeDTO()
            {
                Name = null
            }, "The name property is required to be filled out"
        );
        //Name is empty
        yield return new TestCaseData(new UpdateDeviceTypeDTO()
            {
                Name = ""
            }, "The name property is required to be filled out"
        );
    }
    
    [Test, TestCaseSource(nameof(InvalidUpdateDeviceTypeTestData))]
    public void UpdateDeviceTypeAsync_InvalidData_ThrowsBadRequestException(UpdateDeviceTypeDTO invalidDto, string expectedMessage)
    {
        // Arrange
        int deviceTypeId = 1;

        var service = new DeviceTypeService(_mapper, _mockRepository.Object);

        // Act & Assert
        var ex = Assert.ThrowsAsync<BadRequestException>(async () => await service.UpdateDeviceTypeAsync(deviceTypeId, invalidDto));
        Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
    }
    
    #endregion
}