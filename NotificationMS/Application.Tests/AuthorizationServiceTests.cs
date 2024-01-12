using Application.ApplicationServices.Interfaces;
using Application.ApplicationServices;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests
{
    [TestFixture]
    public class AuthorizationServiceTests
    {
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private AuthorizationService _authorizationService;

        [SetUp]
        public void Setup()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _authorizationService = new AuthorizationService(_authenticationServiceMock.Object);
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
