using System.Linq.Expressions;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Misc;
using Application.DTOs.Setting;
using Application.DTOs.SettingValue;
using Application.Exceptions;
using Application.Mappers.AutoMapper;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Application.Tests;

[TestFixture]
public class DeviceSettingsServiceTests
{
    private IMapper _mapper;
    private Mock<IRepository<SettingValue>> _mockRepository;

    [SetUp]
    public void Setup()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new Profiles()); });
        _mapper = mappingConfig.CreateMapper();

        _mockRepository = new Mock<IRepository<SettingValue>>();
    }

    #region Create Device Settings

    [Test]
    public async Task AddSettingToDevice_Success_ReturnsAddedSettingDTO()
    {
        // Arrange
        var newSettingValueDto = new CreateSettingValueDTO
        {
            Value = 12.5f,
            Setting = new CreateSettingDTO
            {
                Name = "New Setting",
                DefaultValue = 25f,
                UnitId = 1
            },
            DeviceId = 1,
            UserId = "auth0TestId"
        };

        // Expected SettingValue returned from repository
        var createdSettingValue = new SettingValue
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Value = newSettingValueDto.Value,
            Setting = new Setting
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = newSettingValueDto.Setting.Name,
                DefaultValue = newSettingValueDto.Setting.DefaultValue,
                Unit = new Unit
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test Unit",
                    Symbol = "TU",
                    Factor = 2f,
                    Offset = 1f
                }
            },
            UpdateStatus = "Test Status",
            DeviceId = newSettingValueDto.DeviceId,
            UserId = newSettingValueDto.UserId
        };

        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<SettingValue>())).ReturnsAsync(createdSettingValue);

        // Expected SettingValue returned from DeviceSettingsService
        var expectedResult = new SettingValueResponseDTO
        {
            Id = createdSettingValue.Id,
            CreatedAt = createdSettingValue.CreatedAt,
            UpdatedAt = createdSettingValue.UpdatedAt,
            Value = createdSettingValue.Value,
            Setting = new SettingResponseDTO
            {
                Id = createdSettingValue.Setting.Id,
                Name = createdSettingValue.Setting.Name,
                DefaultValue = createdSettingValue.Setting.DefaultValue,
                Unit = new UnitDTO
                {
                    Id = createdSettingValue.Setting.Unit.Id,
                    CreatedAt = createdSettingValue.Setting.Unit.CreatedAt,
                    UpdatedAt = createdSettingValue.Setting.Unit.UpdatedAt,
                    Name = createdSettingValue.Setting.Unit.Name,
                    Symbol = createdSettingValue.Setting.Unit.Symbol,
                    Factor = createdSettingValue.Setting.Unit.Factor,
                    Offset = createdSettingValue.Setting.Unit.Offset
                },
                CreatedAt = createdSettingValue.Setting.CreatedAt,
                UpdatedAt = createdSettingValue.Setting.UpdatedAt
            },
            UpdateStatus = createdSettingValue.UpdateStatus,
            DeviceId = createdSettingValue.DeviceId,
            UserId = createdSettingValue.UserId
        };

        var service = new DeviceSettingsService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.AddSettingToDevice(newSettingValueDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedResult.Id));
            Assert.That(result.CreatedAt, Is.EqualTo(expectedResult.CreatedAt));
            Assert.That(result.UpdatedAt, Is.EqualTo(expectedResult.UpdatedAt));
            Assert.That(result.Value, Is.EqualTo(expectedResult.Value));
            Assert.That(result.Setting?.Id, Is.EqualTo(expectedResult.Setting.Id));
            Assert.That(result.Setting?.Name, Is.EqualTo(expectedResult.Setting.Name));
            Assert.That(result.Setting?.DefaultValue, Is.EqualTo(expectedResult.Setting.DefaultValue));
            Assert.That(result.Setting?.Unit?.Id, Is.EqualTo(expectedResult.Setting.Unit.Id));
            Assert.That(result.Setting?.Unit?.CreatedAt, Is.EqualTo(expectedResult.Setting.Unit.CreatedAt));
            Assert.That(result.Setting?.Unit?.UpdatedAt, Is.EqualTo(expectedResult.Setting.Unit.UpdatedAt));
            Assert.That(result.Setting?.Unit?.Name, Is.EqualTo(expectedResult.Setting.Unit.Name));
            Assert.That(result.Setting?.Unit?.Symbol, Is.EqualTo(expectedResult.Setting.Unit.Symbol));
            Assert.That(result.Setting?.Unit?.Factor, Is.EqualTo(expectedResult.Setting.Unit.Factor));
            Assert.That(result.Setting?.Unit?.Offset, Is.EqualTo(expectedResult.Setting.Unit.Offset));
            Assert.That(result.Setting?.CreatedAt, Is.EqualTo(expectedResult.Setting.CreatedAt));
            Assert.That(result.Setting?.UpdatedAt, Is.EqualTo(expectedResult.Setting.UpdatedAt));
            Assert.That(result.UpdateStatus, Is.EqualTo(expectedResult.UpdateStatus));
            Assert.That(result.DeviceId, Is.EqualTo(expectedResult.DeviceId));
            Assert.That(result.UserId, Is.EqualTo(expectedResult.UserId));
        });
    }

    public static IEnumerable<TestCaseData> InvalidCreateSettingValueTestData()
    {
        // Value is 0
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 0,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Value cannot be negative or 0"
        );
        // Value is negative
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = -10,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Value cannot be negative or 0"
        );
        // Device id is 0
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 0,
                UserId = "auth0TestId"
            }, "Device id cannot be negative or 0"
        );
        // Device id is negative
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = -10,
                UserId = "auth0TestId"
            }, "Device id cannot be negative or 0"
        );
        // Device id is 0
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 0,
                UserId = "auth0TestId"
            }, "Device id cannot be negative or 0"
        );
        // User id is null
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = null
            }, "User id must be specified"
        );
        // User id is empty
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = ""
            }, "User id must be specified"
        );
        // Setting is null
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = null,
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting cannot be null"
        );
        // Setting.Name is null
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = null,
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting Error: Name not specified"
        );
        // Setting.Name is empty
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "",
                    DefaultValue = 25f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting Error: Name not specified"
        );
        // Setting.DefaultValue is negative
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = -10f,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting Error: DefaultValue not specified or negative"
        );
        // Setting.DefaultValue is 0
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 0,
                    UnitId = 1
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting Error: DefaultValue not specified or negative"
        );
        // Setting.UnitId is negative
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = -10
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting Error: UnitId not specified or negative"
        );
        // Setting.UnitId is 0
        yield return new TestCaseData(new CreateSettingValueDTO
            {
                Value = 12.5f,
                Setting = new CreateSettingDTO
                {
                    Name = "New Setting",
                    DefaultValue = 25f,
                    UnitId = 0
                },
                DeviceId = 1,
                UserId = "auth0TestId"
            }, "Setting Error: UnitId not specified or negative"
        );
    }
    
    [Test, TestCaseSource(nameof(InvalidCreateSettingValueTestData))]
    public void AddSettingToDevice_InvalidData_ThrowsBadRequestException(CreateSettingValueDTO invalidSettingvalueDto,
        string expectedMessage)
    {
        var service = new DeviceSettingsService(_mapper, _mockRepository.Object);

        var ex = Assert.ThrowsAsync<BadRequestException>(async () =>
            await service.AddSettingToDevice(invalidSettingvalueDto));
        Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
    }

    #endregion

    #region Retrieve List of Device Settings

    [Test]
    public async Task GetSettingsForDeviceAsync_ExistingDeviceWithSettings_ReturnsSettings()
    {
        // Arrange
        int deviceId = 1;
        var settingValues = new List<SettingValue>
        {
            new SettingValue
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Value = 12.5f,
                Setting = new Setting
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test Setting",
                    DefaultValue = 25f,
                    Unit = new Unit
                    {
                        Id = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Name = "Test Unit",
                        Symbol = "TU",
                        Factor = 2f,
                        Offset = 1f
                    }
                },
                UpdateStatus = "Test Status",
                DeviceId = 1,
                UserId = "auth0TestId"
            },
            new SettingValue
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Value = 25f,
                Setting = new Setting
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test Setting",
                    DefaultValue = 25f,
                    Unit = new Unit
                    {
                        Id = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Name = "Test Unit",
                        Symbol = "TU",
                        Factor = 1f,
                        Offset = 1f
                    }
                },
                UpdateStatus = "Test Status",
                DeviceId = 1,
                UserId = "auth0TestId"
            },
            new SettingValue
            {
                Id = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Value = 12.5f,
                Setting = new Setting
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test Setting",
                    DefaultValue = 50f,
                    Unit = new Unit
                    {
                        Id = 2,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Name = "Test Unit 2",
                        Symbol = "TU2",
                        Factor = 4f,
                        Offset = 1f
                    }
                },
                UpdateStatus = "Test Status 2",
                DeviceId = 2,
                UserId = "auth0TestId2"
            },
        };

        _mockRepository.Setup(repo =>
                repo.GetCollectionByConditionAsync(It.IsAny<Expression<Func<SettingValue, bool>>>()))
            .ReturnsAsync(settingValues);

        var service = new DeviceSettingsService(_mapper, _mockRepository.Object);

        // Act
        var result = await service.GetSettingsForDeviceAsync(deviceId);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(settingValues.Count));
    }

    [Test]
    public void GetSettingsForDeviceAsync_DeviceWithNoSettings_ThrowsNotFoundException()
    {
        // Arrange
        int deviceId = 2;
        var emptySettingsList = new List<SettingValue>(); // Empty SettingValues List

        _mockRepository.Setup(repo =>
                repo.GetCollectionByConditionAsync(It.IsAny<Expression<Func<SettingValue, bool>>>()))
            .ReturnsAsync(emptySettingsList);

        var service = new DeviceSettingsService(_mapper, _mockRepository.Object);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(async () => await service.GetSettingsForDeviceAsync(deviceId));
        Assert.That(ex.Message, Is.EqualTo($"No settings found for Device with ID: {deviceId}"));
    }

    #endregion
}