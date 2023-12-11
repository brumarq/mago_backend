using Moq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs;

namespace Application.Tests;

[TestFixture]
public class Auth0RolesServiceTests
{
    private Mock<IAuth0ManagementService> _mockAuth0ManagementService;
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<Auth0RolesService>> _mockLogger;
    private IAuth0RolesService _auth0RolesService;

    [SetUp]
    public void SetUp()
    {
        _mockAuth0ManagementService = new Mock<IAuth0ManagementService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<Auth0RolesService>>();
        _auth0RolesService = new Auth0RolesService(_mockLogger.Object, _mockAuth0ManagementService.Object, _mockHttpClientFactory.Object, _mockConfiguration.Object);
    }
    
    [Test]
    public async Task UnassignRoleAsync_SuccessfulUnassignment_DoesNotThrowException()
    {
        // Arrange
        var roleId = "adminRoleId";
        var userId = "userId";
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns(roleId);
        var token = new ManagementToken { Token = "validToken", ExpirationTime = DateTime.UtcNow.AddHours(1) };
        _mockAuth0ManagementService.Setup(s => s.GetToken()).ReturnsAsync(token);
        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse("https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/" + userId + "/roles",
            new HttpResponseMessage(HttpStatusCode.NoContent));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _auth0RolesService.UnassignRoleAsync("admin", userId));
    }
    
    [Test]
    public async Task UnassignRoleAsync_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        var roleId = "adminRoleId";
        var userId = "userId";
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns(roleId);
        var token = new ManagementToken { Token = "validToken", ExpirationTime = DateTime.UtcNow.AddHours(1) };
        _mockAuth0ManagementService.Setup(s => s.GetToken()).ReturnsAsync(token);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        Assert.ThrowsAsync<Auth0Service.UserRoleException>(() => _auth0RolesService.UnassignRoleAsync("admin", userId));
    }
    
    [Test]
    public async Task AssignRole_SuccessfulAssignment_DoesNotThrowException()
    {
        var roleId = "adminRoleId";
        var userId = "userId";
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns(roleId);
        var token = new ManagementToken { Token = "validToken", ExpirationTime = DateTime.UtcNow.AddHours(1) };
        _mockAuth0ManagementService.Setup(s => s.GetToken()).ReturnsAsync(token);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/roles/{roleId}/users",
            new HttpResponseMessage(HttpStatusCode.NoContent));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        Assert.DoesNotThrowAsync(() => _auth0RolesService.AssignRole("admin", userId));
    }
    
    [Test]
    public async Task AssignRole_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        var roleId = "adminRoleId";
        var userId = "userId";
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns(roleId);
        var token = new ManagementToken { Token = "validToken", ExpirationTime = DateTime.UtcNow.AddHours(1) };
        _mockAuth0ManagementService.Setup(s => s.GetToken()).ReturnsAsync(token);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/roles/{roleId}/users",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        Assert.ThrowsAsync<Auth0Service.UserRoleException>(() => _auth0RolesService.AssignRole("admin", userId));
    }
    
    [Test]
    public async Task GetRole_SuccessfulRetrieval_ReturnsRoleName()
    {
        var userId = "userId";
        var expectedRoleName = "admin";
        var roles = new List<Role> { new Role { Name = expectedRoleName } };
        var token = new ManagementToken { Token = "validToken", ExpirationTime = DateTime.UtcNow.AddHours(1) };
        _mockAuth0ManagementService.Setup(s => s.GetToken()).ReturnsAsync(token);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(roles)
            });
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        var result = await _auth0RolesService.GetRole(userId);

        Assert.AreEqual(expectedRoleName, result);
    }

    [Test]
    public async Task GetRole_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        var userId = "userId";
        var token = new ManagementToken { Token = "validToken", ExpirationTime = DateTime.UtcNow.AddHours(1) };
        _mockAuth0ManagementService.Setup(s => s.GetToken()).ReturnsAsync(token);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        Assert.ThrowsAsync<Auth0Service.UserRoleException>(() => _auth0RolesService.GetRole(userId));
    }




    
    private class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses = new Dictionary<string, HttpResponseMessage>();

        public void SetupResponse(string url, HttpResponseMessage responseMessage)
        {
            _responses[url] = responseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_responses.TryGetValue(request.RequestUri.ToString(), out var response))
            {
                return await Task.FromResult(response);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Not Found") };
        }
    }
}
