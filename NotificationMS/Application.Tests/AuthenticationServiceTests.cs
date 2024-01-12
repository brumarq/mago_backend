using Application.ApplicationServices;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace Application.Tests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());

            _authenticationService = new AuthenticationService(_httpContextAccessorMock.Object);
        }

        [Test]
        public void GetUserId_Should_ReturnUserId_When_UserIsAuthenticated()
        {
            var userId = "userid123";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessorMock.Setup(a => a.HttpContext.User).Returns(principal);

            var result = _authenticationService.GetUserId();

            Assert.AreEqual(userId, result);
        }

        [Test]
        public void GetUserId_Should_ReturnNull_When_UserIsNotAuthenticated()
        {
            _httpContextAccessorMock.Setup(a => a.HttpContext.User).Returns(new ClaimsPrincipal());

            var result = _authenticationService.GetUserId();

            Assert.IsNull(result);
        }

        [Test]
        public void HasPermission_Should_ReturnTrue_When_UserHasSpecifiedPermission()
        {
            var permission = "test-permission";
            var claims = new List<Claim> { new Claim("permissions", permission) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessorMock.Setup(a => a.HttpContext.User).Returns(principal);

            var result = _authenticationService.HasPermission(permission);

            Assert.IsTrue(result);
        }

        [Test]
        public void HasPermission_Should_ReturnFalse_When_UserDoesNotHaveSpecifiedPermission()
        {
            var claims = new List<Claim> { new Claim("permissions", "other-permission") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _httpContextAccessorMock.Setup(a => a.HttpContext.User).Returns(principal);

            var result = _authenticationService.HasPermission("test-permission");

            Assert.IsFalse(result);
        }

        [Test]
        public void GetToken_Should_ReturnToken_When_AuthorizationHeaderIsPresent()
        {
            var token = "test-token";
            _httpContextAccessorMock.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns($"Bearer {token}");

            var result = _authenticationService.GetToken();

            Assert.AreEqual(token, result);
        }

        [Test]
        public void GetToken_Should_ThrowBadRequestException_When_AuthorizationHeaderIsMissing()
        {
            _httpContextAccessorMock.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns(string.Empty);

            Assert.Throws<BadRequestException>(() => _authenticationService.GetToken());
        }

        [Test]
        public void IsLoggedInUser_Should_ReturnTrue_When_UserHasClientPermission()
        {
            var claims = new List<Claim> { new Claim("permissions", "client") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _httpContextAccessorMock.Setup(a => a.HttpContext.User).Returns(principal);

            var result = _authenticationService.IsLoggedInUser();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsLoggedInUser_Should_ReturnTrue_When_UserHasAdminPermission()
        {
            var claims = new List<Claim> { new Claim("permissions", "admin") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _httpContextAccessorMock.Setup(a => a.HttpContext.User).Returns(principal);

            var result = _authenticationService.IsLoggedInUser();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsLoggedInUser_ShouldReturnFalse_When_UserHasNoRelevantPermissions()
        {
            var claims = new List<Claim> { new Claim("permissions", "other") };
            var identity = new ClaimsIdentity(claims);
            _httpContextAccessorMock.Setup(a => a.HttpContext.User.Identity).Returns(identity);

            var result = _authenticationService.IsLoggedInUser();

            Assert.IsFalse(result);
        }

    }
}
