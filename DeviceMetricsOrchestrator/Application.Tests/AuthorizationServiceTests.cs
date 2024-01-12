using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.UsersOnDevices;
using Moq;

namespace Application.Tests
{
    [TestFixture]
    public class AuthorizationServiceTests
    {
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<IUsersOnDevicesService> _usersOnDevicesServiceMock;
        private AuthorizationService _authorizationService;

        [SetUp]
        public void Setup()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _usersOnDevicesServiceMock = new Mock<IUsersOnDevicesService>();
            _authorizationService = new AuthorizationService(_authenticationServiceMock.Object, _usersOnDevicesServiceMock.Object);
        }

        [Test]
        public async Task IsDeviceAccessibleToUser_Should_ReturnTrue_When_UserIsAdmin()
        {
            _authenticationServiceMock.Setup(a => a.HasPermission("admin")).Returns(true);

            var result = await _authorizationService.IsDeviceAccessibleToUser("anyUserId", 123);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsDeviceAccessibleToUser_Should_ReturnTrue_When_DeviceBelongsToUser()
        {
            _authenticationServiceMock.Setup(a => a.HasPermission("admin")).Returns(false);
            var loggedInUserId = "userId123";
            var deviceId = 1;

            var device = new DeviceResponseDTO
            {
                Id = deviceId,
                Name = "Device 1",
                DeviceType = new DeviceTypeResponseDTO { Id = 1, Name = "Device type 1" },
                SendSettingsAtConn = true,
                SendSettingsNow = false,
                AuthId = "Auth123"
            };

            _usersOnDevicesServiceMock.Setup(uod => uod.GetUsersOnDevicesByUserIdAsync(loggedInUserId))
                .ReturnsAsync(new List<UsersOnDevicesResponseDTO> { new UsersOnDevicesResponseDTO { Device = device } });

            var result = await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId, deviceId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsDeviceAccessibleToUser_Should_ReturnFalse_When_DeviceDoesNotBelongToUserAndUserIsNotAdmin()
        {
            _authenticationServiceMock.Setup(a => a.HasPermission("admin")).Returns(false);
            var loggedInUserId = "userId123";
            var deviceId = 1;

            _usersOnDevicesServiceMock.Setup(uod => uod.GetUsersOnDevicesByUserIdAsync(loggedInUserId))
                .ReturnsAsync(new List<UsersOnDevicesResponseDTO>());

            var result = await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId, deviceId);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsCorrectUserOrAdmin_Should_ReturnTrue_When_UserIsAdmin()
        {
            _authenticationServiceMock.Setup(a => a.HasPermission("admin")).Returns(true);

            var result = _authorizationService.IsCorrectUserOrAdmin("anyUserId", "differentUserId");

            Assert.IsTrue(result);
        }

        [Test]
        public void IsCorrectUserOrAdmin_Should_ReturnTrue_When_UserIsCorrectUser()
        {
            _authenticationServiceMock.Setup(a => a.HasPermission("admin")).Returns(false);

            var userId = "userId123";
            var result = _authorizationService.IsCorrectUserOrAdmin(userId, userId);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsCorrectUserOrAdmin_Should_ReturnFalse_When_UserIsNotAdminNorCorrectUser()
        {
            _authenticationServiceMock.Setup(a => a.HasPermission("admin")).Returns(false);

            var result = _authorizationService.IsCorrectUserOrAdmin("userId123", "differentUserId");

            Assert.IsFalse(result);
        }
    }
}