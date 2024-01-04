using System.Linq.Expressions;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.DeviceType;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using Application.Mappers.AutoMapper;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Application.Tests;

[TestFixture]
public class UsersOnDevicesServiceTests
{
    private IMapper _mapper;
    private Mock<IRepository<UsersOnDevices>> _mockUsersOnDevicesRepository;
    private Mock<IDeviceService> _mockDeviceService;
    private Mock<IRepository<Device>> _mockDeviceRepository;

    [SetUp]
    public void Setup()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new Profiles()); });
        _mapper = mappingConfig.CreateMapper();

        _mockUsersOnDevicesRepository = new Mock<IRepository<UsersOnDevices>>();
        _mockDeviceService = new Mock<IDeviceService>();
        _mockDeviceRepository = new Mock<IRepository<Device>>();
    }

    #region Create UserOnDevice Tests

    [Test]
    public async Task CreateUserOnDeviceAsync_Success_ReturnsCreateUserOnDeviceDTO()
    {
        // Arrange
        var createUserOnDeviceDTO = new CreateUserOnDeviceDTO
        {
            UserId = "testUserId",
            DeviceId = 1
        };

        var expectedDevice = new Device
        {
            Id = createUserOnDeviceDTO.DeviceId,
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

        _mockDeviceRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .ReturnsAsync(expectedDevice);

        // Expected device response from device service
        var expectedDeviceResponseDto = new DeviceResponseDTO()
        {
            Id = createUserOnDeviceDTO.DeviceId,
            Name = expectedDevice.Name,
            DeviceType = new DeviceTypeResponseDTO
            {
                Id = expectedDevice.DeviceType.Id,
                CreatedAt = expectedDevice.DeviceType.CreatedAt,
                UpdatedAt = expectedDevice.DeviceType.UpdatedAt,
                Name = expectedDevice.DeviceType.Name
            },
            SendSettingsAtConn = expectedDevice.SendSettingsAtConn,
            SendSettingsNow = expectedDevice.SendSettingsNow,
            AuthId = expectedDevice.AuthId,
            CreatedAt = expectedDevice.CreatedAt,
            UpdatedAt = expectedDevice.UpdatedAt
        };

        _mockDeviceService.Setup(service => service.GetDeviceByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedDeviceResponseDto);

        var expectedUserOnDevice = new UsersOnDevices
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeviceId = expectedDevice.Id,
            Device = expectedDevice
        };

        _mockUsersOnDevicesRepository.Setup(repo => repo.CreateAsync(It.IsAny<UsersOnDevices>()))
            .ReturnsAsync(expectedUserOnDevice);


        var service =
            new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        // Act
        var result = await service.CreateUserOnDeviceAsync(createUserOnDeviceDTO);

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public void CreateUserOnDeviceAsync_NonExistingDevice_ThrowsNotFoundException()
    {
        // Arrange
        var createUserOnDeviceDTO = new CreateUserOnDeviceDTO
        {
            UserId = "testUserId",
            DeviceId = 999 // Non-existing device ID
        };

        _mockDeviceService.Setup(service => service.GetDeviceByIdAsync(It.IsAny<int>())).ReturnsAsync((DeviceResponseDTO)null);

        var service = new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await service.CreateUserOnDeviceAsync(createUserOnDeviceDTO));
    }
    
    [Test]
    public void CreateUserOnDeviceAsync_UserAlreadyAssignedToDevice_ThrowsBadRequestException()
    {
        // Arrange
        var createUserOnDeviceDTO = new CreateUserOnDeviceDTO
        {
            UserId = "existingUserId",
            DeviceId = 1 // Existing device ID
        };

        var existingUserOnDevice = new UsersOnDevices();

        _mockUsersOnDevicesRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<UsersOnDevices, bool>>>()))
            .ReturnsAsync(existingUserOnDevice);
        _mockDeviceService.Setup(service => service.GetDeviceByIdAsync(It.IsAny<int>())).ReturnsAsync(new DeviceResponseDTO());

        var service = new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await service.CreateUserOnDeviceAsync(createUserOnDeviceDTO));
    }
    
    public static IEnumerable<TestCaseData> InvalidCreateUsersOnDevicesTestData()
    {
        // UserId is null
        yield return new TestCaseData(new CreateUserOnDeviceDTO
            {
                UserId = null,
                DeviceId = 1
            }, "The user id field is required to be filled out."
        );
        // UserId is empty
        yield return new TestCaseData(new CreateUserOnDeviceDTO
            {
                UserId = "",
                DeviceId = 1
            }, "The user id field is required to be filled out."
        );
        // DeviceId is 0
        yield return new TestCaseData(new CreateUserOnDeviceDTO
            {
                UserId = "testUserId",
                DeviceId = 0
            }, "The device id cannot be 0 or negative"
        );
        // DeviceId is negative
        yield return new TestCaseData(new CreateUserOnDeviceDTO
            {
                UserId = "testUserId",
                DeviceId = -10
            }, "The device id cannot be 0 or negative"
        );
    }

    [Test, TestCaseSource(nameof(InvalidCreateUsersOnDevicesTestData))]
    public void CreateDeviceAsync_InvalidData_ThrowsBadRequestException(CreateUserOnDeviceDTO invalidDto,
        string expectedMessage)
    {
        var service = new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        var ex = Assert.ThrowsAsync<BadRequestException>(async () => await service.CreateUserOnDeviceAsync(invalidDto));
        Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
    }
    
    [Test]
    public async Task GetUsersOnDevicesByUserIdAsync_ExistingUserId_ReturnsListOfAssociations()
    {
        // Arrange
        string userId = "existingUserId"; // A user ID with associated devices
        var mockUserOnDevices = new List<UsersOnDevices>
        {
            new UsersOnDevices
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeviceId = 1,
                Device = new Device
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
                }
            },
            new UsersOnDevices
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeviceId = 1,
                Device = new Device
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
                }
            },
        };

        _mockUsersOnDevicesRepository.Setup(repo => repo.GetCollectionByConditionAsync(It.IsAny<Expression<Func<UsersOnDevices, bool>>>()))
            .ReturnsAsync(mockUserOnDevices);

        var service = new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        // Act
        var result = await service.GetUsersOnDevicesByUserIdAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(mockUserOnDevices.Count));
    }
    
    #endregion
    
    #region Delete UserOnDevice Tests

    [Test]
    public async Task DeleteUserOnDeviceAsync_ExistingAssociation_ReturnsTrue()
    {
        // Arrange
        int userOnDeviceId = 1; // Assuming this is an existing user-device association ID

        _mockUsersOnDevicesRepository.Setup(repo => repo.DeleteAsync(userOnDeviceId)).ReturnsAsync(true);

        var service = new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        // Act
        var result = await service.DeleteUserOnDeviceAsync(userOnDeviceId);

        // Assert
        Assert.IsTrue(result);
    }
    
    [Test]
    public async Task DeleteUserOnDeviceAsync_NonExistingAssociation_ReturnsFalse()
    {
        // Arrange
        int nonExistingUserOnDeviceId = 999; // An ID that does not exist

        _mockUsersOnDevicesRepository.Setup(repo => repo.DeleteAsync(nonExistingUserOnDeviceId)).ReturnsAsync(false); // Simulate non-existing deletion

        var service = new UsersOnDevicesService(_mapper, _mockUsersOnDevicesRepository.Object, _mockDeviceService.Object);

        // Act
        var result = await service.DeleteUserOnDeviceAsync(nonExistingUserOnDeviceId);

        // Assert
        Assert.IsFalse(result);
    }
    
    #endregion
}