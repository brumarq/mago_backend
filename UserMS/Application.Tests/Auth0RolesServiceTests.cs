using Moq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using Domain.Entities;

namespace Application.Tests;

[TestFixture]
public class Auth0RolesServiceTests
{
    private Mock<IAuth0ManagementService> _mockAuth0ManagementService = null!;
    private Mock<IHttpClientFactory> _mockHttpClientFactory = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<ILogger<Auth0RolesService>> _mockLogger = null!;
    private IAuth0RolesService _auth0RolesService = null!;
    private FakeHttpMessageHandler _fakeHttpMessageHandler = null!;
    private HttpClient _fakeHttpClient = null!;

    [SetUp]
    public void SetUp()
    {
        _mockAuth0ManagementService = new Mock<IAuth0ManagementService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<Auth0RolesService>>();
        _auth0RolesService = new Auth0RolesService(_mockLogger.Object, _mockAuth0ManagementService.Object, _mockHttpClientFactory.Object, _mockConfiguration.Object);
        
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns("adminRoleId");
        _mockConfiguration.Setup(c => c["Auth0-Management:Audience"]).Returns("https://random.com/api/v2/");
        
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(new ManagementToken { Token = "mockToken" });
        _fakeHttpMessageHandler = new FakeHttpMessageHandler();
        _fakeHttpClient = new HttpClient(_fakeHttpMessageHandler);
    }
    
    [TearDown]
    public void TearDown()
    {
        _fakeHttpMessageHandler.Dispose();
        _fakeHttpClient.Dispose();
    }
    
    [Test]
    public void UnassignRoleAsync_SuccessfulUnassignment_DoesNotThrowException()
    {
        // Arrange
        const string userId = "userId";
        
        _fakeHttpMessageHandler.SetupResponse("https://random.com/api/v2/users/" + userId + "/roles",
            new HttpResponseMessage(HttpStatusCode.NoContent));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _auth0RolesService.UnassignRoleAsync("admin", userId));
    }
    
    [Test]
    public void UnassignRoleAsync_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        const string userId = "userId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        Assert.ThrowsAsync<BadRequestException>(() => _auth0RolesService.UnassignRoleAsync("admin", userId));
    }
    
    [Test]
    public void AssignRole_SuccessfulAssignment_DoesNotThrowException()
    {
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/roles/adminRoleId/users",
            new HttpResponseMessage(HttpStatusCode.NoContent));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        Assert.DoesNotThrowAsync(() => _auth0RolesService.AssignRole("admin", "userId"));
    }
    
    [Test]
    public void AssignRole_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        const string roleId = "adminRoleId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/roles/{roleId}/users",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        Assert.ThrowsAsync<BadRequestException>(() => _auth0RolesService.AssignRole("admin", "userId"));
    }
    
    [Test]
    public async Task GetRole_SuccessfulRetrieval_ReturnsRoleName()
    {
        const string expectedRoleName = "admin";
        
        var roles = new List<Role> { new Role { Name = expectedRoleName } };
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/userId/roles",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(roles)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        var result = await _auth0RolesService.GetRole("userId");

        Assert.That(result, Is.EqualTo(expectedRoleName));
    }

    [Test]
    public void GetRole_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        const string userId = "userId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        Assert.ThrowsAsync<BadRequestException>(() => _auth0RolesService.GetRole(userId));
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
