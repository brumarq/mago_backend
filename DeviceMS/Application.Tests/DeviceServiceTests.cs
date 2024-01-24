using System.Linq.Expressions;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.DeviceType;
using Application.Exceptions;
using Application.Mappers.AutoMapper;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Application.Tests;

[TestFixture]
public class DeviceServiceTests
{
    private IMapper _mapper;
    private Mock<IDeviceTypeService> _mockDeviceTypeService;
    private Mock<IRepository<Device>> _mockRepository;

    [SetUp]
    public void Setup()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new Profiles()); });
        _mapper = mappingConfig.CreateMapper();

        _mockDeviceTypeService = new Mock<IDeviceTypeService>();
        _mockRepository = new Mock<IRepository<Device>>();
    }

    #region Create Device Tests

    [Test]
    public async Task CreateDeviceAsync_Success_ReturnsDeviceResponseDTO()
    {
        // Arrange
        var newDeviceDto = new CreateDeviceDTO
        {
            Name = "Test Name",
            DeviceTypeId = 1,
            SendSettingsAtConn = true,
            SendSettingsNow = true,
            AuthId = "string",
            Password = "string"
        };

        // Expected DeviceType returned from DeviceTypeService
        var deviceTypeResponse = new DeviceTypeResponseDTO
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Test Device Type"
        };

        // Setup device type response
        _mockDeviceTypeService.Setup(service =>
                service.GetDeviceTypeByIdAsync(It.Is<int>(id => id == newDeviceDto.DeviceTypeId)))
            .ReturnsAsync(deviceTypeResponse);

        // Expected Device returned from repository
        var createdDevice = new Device
        {
            Id = 1,
            Name = "Test Name",
            DeviceTypeId = 1,
            DeviceType = new DeviceType
            {
                Id = deviceTypeResponse.Id,
                CreatedAt = deviceTypeResponse.CreatedAt,
                UpdatedAt = deviceTypeResponse.UpdatedAt,
                Name = deviceTypeResponse.Name
            },
            SendSettingsAtConn = true,
            SendSettingsNow = true,
            AuthId = "string",
            PwHash = "string",
            Salt = "string",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Setup device repository response
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Device>())).ReturnsAsync(createdDevice);

        var expectedResult = new DeviceResponseDTO
        {
            Id = createdDevice.Id,
            Name = createdDevice.Name,
            DeviceType = deviceTypeResponse,
            SendSettingsAtConn = createdDevice.SendSettingsAtConn,
            SendSettingsNow = createdDevice.SendSettingsNow,
            AuthId = createdDevice.AuthId,
            CreatedAt = createdDevice.CreatedAt,
            UpdatedAt = createdDevice.UpdatedAt
        };

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.CreateDeviceAsync(newDeviceDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedResult.Id));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
            Assert.Multiple(() =>
            {
                Assert.That(result.DeviceType?.Id, Is.EqualTo(expectedResult.DeviceType.Id));
                Assert.That(result.DeviceType?.CreatedAt, Is.EqualTo(expectedResult.DeviceType.CreatedAt));
                Assert.That(result.DeviceType?.UpdatedAt, Is.EqualTo(expectedResult.DeviceType.UpdatedAt));
                Assert.That(result.DeviceType?.Name, Is.EqualTo(expectedResult.DeviceType.Name));
            });
            Assert.That(result.SendSettingsAtConn, Is.EqualTo(expectedResult.SendSettingsAtConn));
            Assert.That(result.SendSettingsNow, Is.EqualTo(expectedResult.SendSettingsNow));
            Assert.That(result.AuthId, Is.EqualTo(expectedResult.AuthId));
            Assert.That(result.CreatedAt, Is.EqualTo(expectedResult.CreatedAt));
            Assert.That(result.UpdatedAt, Is.EqualTo(expectedResult.UpdatedAt));
        });
    }

    [Test]
    public void CreateDeviceAsync_NonExistingDeviceType_ThrowsNotFoundException()
    {
        // Arrange
        var newDeviceDto = new CreateDeviceDTO
        {
            Name = "Test Name",
            DeviceTypeId = 999, // id does not exist
            SendSettingsAtConn = true,
            SendSettingsNow = true,
            AuthId = "string",
            Password = "string"
        };

        _mockDeviceTypeService.Setup(service =>
                service.GetDeviceTypeByIdAsync(It.Is<int>(id => id == newDeviceDto.DeviceTypeId)))
            .ReturnsAsync((DeviceTypeResponseDTO)null); // Return null for non-existing device type

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await service.CreateDeviceAsync(newDeviceDto));

        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Device>()),
            Times.Never()); // Verify no device is created
    }

    public static IEnumerable<TestCaseData> InvalidCreateDeviceTestData()
    {
        // Object is null
        yield return new TestCaseData(null, "The object cannot be null."
        );
        // Name is null
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = null,
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'Name' property is required to be filled out"
        );
        // Name is empty
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'Name' property is required to be filled out"
        );
        // DeviceTypeId < 0
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = -10,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'DeviceTypeId' property cannot be negative or 0."
        );
        // DeviceTypeId == 0
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 0,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'DeviceTypeId' property cannot be negative or 0."
        );
        // AuthId is null
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = null,
                Password = "string"
            }, "The 'AuthId' property is required to be filled out"
        );
        // AuthId is empty
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "",
                Password = "string"
            }, "The 'AuthId' property is required to be filled out"
        );
        // Password is null
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = null
            }, "The 'Password' property is required to be filled out"
        );
        // Password is empty
        yield return new TestCaseData(new CreateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = ""
            }, "The 'Password' property is required to be filled out"
        );
    }

    [Test, TestCaseSource(nameof(InvalidCreateDeviceTestData))]
    public void CreateDeviceAsync_InvalidData_ThrowsBadRequestException(CreateDeviceDTO invalidDto,
        string expectedMessage)
    {
        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        var ex = Assert.ThrowsAsync<BadRequestException>(async () => await service.CreateDeviceAsync(invalidDto));
        Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
    }

    #endregion

    #region Retrieve List of Devices Tests

    [Test]
    public async Task GetDevicesAsync_ReturnsListOfDevices()
    {
        // Arrange
        var devices = new List<Device>
        {
            new Device
            {
                Id = 1,
                Name = "Test Device",
                DeviceTypeId = 1,
                DeviceType = new DeviceType
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test device type"
                },
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                PwHash = "string",
                Salt = "string",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new Device
            {
                Id = 2,
                Name = "Test Device 2",
                DeviceTypeId = 1,
                DeviceType = new DeviceType
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test device type"
                },
                SendSettingsAtConn = false,
                SendSettingsNow = false,
                AuthId = "string",
                PwHash = "string",
                Salt = "string",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new Device
            {
                Id = 3,
                Name = "Test Device 3",
                DeviceTypeId = 2,
                DeviceType = new DeviceType
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Name = "Test device type 2"
                },
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                PwHash = "string",
                Salt = "string",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(devices);

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.GetDevicesAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(devices.Count));
    }

    [Test]
    public async Task GetDevicesAsync_NoDevices_ReturnsListOfDevices()
    {
        // Arrange
        var emptyDeviceList = new List<Device>(); // Empty list of devices

        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(emptyDeviceList);

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.GetDevicesAsync();

        // Assert
        Assert.IsNotNull(result); // Ensure the result is not null
        Assert.IsEmpty(result); // Ensure the result is an empty collection
    }

    #endregion

    #region Retrieve Specific Device Tests

    [Test]
    public async Task GetDeviceByIdAsync_ExistingDevice_ReturnsDeviceResponseDTO()
    {
        // Arrange
        var expectedDevice = new Device
        {
            Id = 1,
            Name = "Test Device",
            DeviceTypeId = 1,
            DeviceType = new DeviceType
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Test device type"
            },
            SendSettingsAtConn = true,
            SendSettingsNow = true,
            AuthId = "string",
            PwHash = "string",
            Salt = "string",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        int deviceId = expectedDevice.Id;

        _mockRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .ReturnsAsync(expectedDevice);

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.GetDeviceByIdAsync(deviceId);

        // Assert
        Assert.IsNotNull(result);
    }
    
    [Test]
    public async Task GetDeviceByIdAsync_NonExistingDevice_ReturnsNull()
    {
        // Arrange
        int nonExistingDeviceId = -1; // Id that does not exist

        _mockRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .ReturnsAsync((Device)null);

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.GetDeviceByIdAsync(nonExistingDeviceId);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Update Device Tests

    [Test]
    public async Task UpdateDeviceAsync_ValidData_ReturnsTrue()
    {
        // Arrange
        var updateDto = new UpdateDeviceDTO
        {
            Name = "Test Name",
            DeviceTypeId = 1,
            SendSettingsAtConn = true,
            SendSettingsNow = true,
            AuthId = "string",
            Password = "string"
        };
        int deviceId = 1; // Assuming this is an existing device ID

        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Device>()))
            .ReturnsAsync(true);

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.UpdateDeviceAsync(deviceId, updateDto);

        // Assert
        Assert.IsTrue(result);
    }
    
    [Test]
    public async Task UpdateDeviceAsync_NonExistingDevice_ReturnsFailure()
    {
        // Arrange
        var updateDto = new UpdateDeviceDTO
        {
            Name = "Test Name",
            DeviceTypeId = 1,
            SendSettingsAtConn = true,
            SendSettingsNow = true,
            AuthId = "string",
            Password = "string"
        };
        int nonExistingDeviceId = -1;

        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Device>()))
            .ReturnsAsync(false);

        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act
        var result = await service.UpdateDeviceAsync(nonExistingDeviceId, updateDto);

        // Assert
        Assert.IsFalse(result);
    }
    
    public static IEnumerable<TestCaseData> InvalidUpdateDeviceTestData()
    {
        // Object is null
        yield return new TestCaseData(null, "The object cannot be null."
        );
        // Name is null
        yield return new TestCaseData(new UpdateDeviceDTO()
            {
                Name = null,
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'Name' property is required to be filled out"
        );
        // Name is empty
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'Name' property is required to be filled out"
        );
        // DeviceTypeId < 0
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = -10,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'DeviceTypeId' property cannot be negative or 0."
        );
        // DeviceTypeId == 0
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 0,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = "string"
            }, "The 'DeviceTypeId' property cannot be negative or 0."
        );
        // AuthId is null
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = null,
                Password = "string"
            }, "The 'AuthId' property is required to be filled out"
        );
        // AuthId is empty
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "",
                Password = "string"
            }, "The 'AuthId' property is required to be filled out"
        );
        // Password is null
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = null
            }, "The 'Password' property is required to be filled out"
        );
        // Password is empty
        yield return new TestCaseData(new UpdateDeviceDTO
            {
                Name = "Test Name",
                DeviceTypeId = 1,
                SendSettingsAtConn = true,
                SendSettingsNow = true,
                AuthId = "string",
                Password = ""
            }, "The 'Password' property is required to be filled out"
        );
    }

    [Test, TestCaseSource(nameof(InvalidUpdateDeviceTestData))]
    public void UpdateDeviceAsync_InvalidData_ThrowsBadRequestException(UpdateDeviceDTO invalidDto, string expectedMessage)
    {
        // Arrange
        int deviceId = 1; // Assuming this is an existing device ID
        
        var service = new DeviceService(_mapper, _mockRepository.Object, _mockDeviceTypeService.Object);

        // Act & Assert
        var ex = Assert.ThrowsAsync<BadRequestException>(async () => await service.UpdateDeviceAsync(deviceId, invalidDto));
        Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
    }
    
    #endregion
}