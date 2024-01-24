using Moq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class Auth0RolesServiceTests
{
    // Mock objects for dependencies of Auth0RolesService
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
        // Initializing the mock objects
        _mockAuth0ManagementService = new Mock<IAuth0ManagementService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<Auth0RolesService>>();

        // Setting up the Auth0RolesService with mocked dependencies
        _auth0RolesService = new Auth0RolesService(_mockLogger.Object, _mockAuth0ManagementService.Object, _mockHttpClientFactory.Object, _mockConfiguration.Object);
        
        // Configuring mock responses for role IDs and management audience
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns("adminRoleId");
        _mockConfiguration.Setup(c => c["Auth0-Management:Audience"]).Returns("https://random.com/api/v2/");
        
        // Mocking the token retrieval for Auth0 management service
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(new ManagementToken { Token = "mockToken" });

        // Setting up the fake HTTP handler and client to mock HTTP requests
        _fakeHttpMessageHandler = new FakeHttpMessageHandler();
        _fakeHttpClient = new HttpClient(_fakeHttpMessageHandler);
    }
    
    [TearDown]
    public void TearDown()
    {
        // Disposing the fake HTTP handler and client after each test to ensure clean state
        _fakeHttpMessageHandler.Dispose();
        _fakeHttpClient.Dispose();
    }
    
    [Test]
    public void UnassignRoleAsync_SuccessfulUnassignment_DoesNotThrowException()
    {
        // Arrange: Set up a successful HTTP response for role unassignment
        const string userId = "userId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.NoContent));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Ensure that unassigning a role does not throw an exception
        Assert.DoesNotThrowAsync(() => _auth0RolesService.UnassignRoleAsync("admin", userId));
    }
    
    [Test]
    public void UnassignRoleAsync_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        // Arrange: Mock an unsuccessful HTTP response for role unassignment
        const string userId = "userId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Verify that a BadRequestException is thrown for an unsuccessful operation
        Assert.ThrowsAsync<BadRequestException>(() => _auth0RolesService.UnassignRoleAsync("admin", userId));
    }

    
    [Test]
    public void AssignRole_SuccessfulAssignment_DoesNotThrowException()
    {
        // Arrange: Mock a successful HTTP response for role assignment
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/roles/adminRoleId/users",
            new HttpResponseMessage(HttpStatusCode.NoContent));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Check that assigning a role does not throw an exception
        Assert.DoesNotThrowAsync(() => _auth0RolesService.AssignRole("admin", "userId"));
    }

    
    [Test]
    public void AssignRole_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        // Arrange: Mock an unsuccessful HTTP response for role assignment
        const string roleId = "adminRoleId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/roles/{roleId}/users",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Verify that a BadRequestException is thrown for an unsuccessful operation
        Assert.ThrowsAsync<BadRequestException>(() => _auth0RolesService.AssignRole("admin", "userId"));
    }

    
    [Test]
    public async Task GetRole_SuccessfulRetrieval_ReturnsRoleName()
    {
        // Arrange: Mock a successful HTTP response for retrieving a role
        const string expectedRoleName = "admin";
        var roles = new List<Role> { new() { Name = expectedRoleName } };

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/userId/roles",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(roles)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act: Attempt to retrieve the role name
        var result = await _auth0RolesService.GetRole("userId");

        // Assert: Verify that the retrieved role name matches the expected value
        Assert.That(result, Is.EqualTo(expectedRoleName));
    }


    [Test]
    public void GetRole_UnsuccessfulResponse_ThrowsUserRoleException()
    {
        // Arrange: Mock an unsuccessful HTTP response for retrieving a role
        const string userId = "userId";
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}/roles",
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Verify that a BadRequestException is thrown for an unsuccessful operation
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
