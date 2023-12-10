using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class Auth0ServiceTests
{
    private Mock<ILogger<UserService>> _mockLogger;
    private Mock<IAuth0ManagementService> _mockAuth0ManagementService;
    private Mock<IAuth0RolesService> _mockAuth0Roles;
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<IConfiguration> _mockConfiguration;
    private Auth0Service _auth0Service;
    private Mock<IAuth0Service> _mockAuth0Service;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockAuth0ManagementService = new Mock<IAuth0ManagementService>();
        _mockAuth0Service = new Mock<IAuth0Service>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockAuth0Roles = new Mock<IAuth0RolesService>();
        
        // Mock configuration for role URLs
        _mockConfiguration.Setup(config => config[It.IsAny<string>()]).Returns("configuredValue");

        _auth0Service = new Auth0Service(_mockLogger.Object, _mockAuth0ManagementService.Object, _mockHttpClientFactory.Object, _mockConfiguration.Object,_mockAuth0Roles.Object);
    }

    [Test]
    public async Task CreateAuth0UserAsync_SuccessfulCreation_ReturnsUser()
    {
        // Arrange
        var createUserDto = new CreateUserDTO {
            Email = "johnDoe@gmail.com",
            FamilyName = "Doe",
            GivenName = "John",
            Password = "StrongPass!23",
            SysAdmin = false
        };
        
        // Mock Token Retrieval
        var mockToken = new ManagementToken { Token = "mockToken" };
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();

        // Mock the response for the user creation
        fakeHttpMessageHandler.SetupResponse("https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users", 
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(JsonConvert.SerializeObject(new {
                    user_id = "auth0|jahsjdkhasjkd",
                    name = "John Doe",
                    family_name = "Doe",
                    given_name = "John",
                    email = "johnDoe@gmail.com",
                    email_verified = false,
                    blocked = false,
                    created_at = "10/12/1222",
                    updated_at = "10/12/1222",
                    picture = "hhtps://samplePicture.com"
                }))
            });

        // Mock the response for the role assignment - Ensure valid JSON content
        var roleAssignmentResponseContent = JsonConvert.SerializeObject(new { result = "success" });
        fakeHttpMessageHandler.SetupResponse("https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/roles/configuredValue/users", 
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(roleAssignmentResponseContent)
            });

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        // Act
        var result = await _auth0Service.CreateAuth0UserAsync(createUserDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.User.Name, Is.EqualTo("John Doe"));
            Assert.That(result.Role, Is.EqualTo("client"));
        });
    }
    
    [Test]
    public void CreateAuth0UserAsync_WrongGivenName_ThrowsInvalidUserDetailsException()
    {
        // Arrange
        var createUserDto = new CreateUserDTO {
            Email = "johnDoe@gmail.com",
            FamilyName = "",
            GivenName = "",
            Password = "StrongPass!23",
            SysAdmin = false
        };
    
        // Act & Assert
        Assert.ThrowsAsync<Auth0Service.InvalidUserDetailsException>(async () => await _auth0Service.CreateAuth0UserAsync(createUserDto));
    }
    
    [Test]
    public void CreateAuth0UserAsync_Auth0UserCreationFails_ThrowsUserCreationException()
    {
        // Arrange
        var createUserDto = new CreateUserDTO {
            Email = "test@example.com",
            FamilyName = "Test",
            GivenName = "User",
            Password = "StrongPass!23",
            SysAdmin = false
        };

        // Mock Token Retrieval
        var mockToken = new ManagementToken { Token = "mockToken" };
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Mock the failure response for the user creation
        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        var errorResponseContent = "Error message";
        fakeHttpMessageHandler.SetupResponse("https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users", 
            new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(errorResponseContent)
            });

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);
        
        
        
        // Mock Logger
        _mockLogger.Setup(logger => 
            logger.Log(
                LogLevel.Error, 
                It.IsAny<EventId>(), 
                It.Is<It.IsAnyType>((v, t) => true), 
                It.IsAny<Exception>(), 
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!)
        ).Verifiable();

        // Act & Assert
        var exception = Assert.ThrowsAsync<Auth0Service.UserCreationException>(async () => await _auth0Service.CreateAuth0UserAsync(createUserDto));
        Assert.That(exception?.Message, Does.Contain("Error creating user in Auth0: " + errorResponseContent));

        // Verify that the logger was called
        _mockLogger.Verify();
    }
    
    // Updating Account
    
    [Test]
    public void UpdateUserAsync_EmailAndPasswordSimultaneously_ThrowsArgumentException()
    {
        var updateUserDto = new UpdateUserDTO {
            FamilyName = "Doe",
            GivenName = "John",
            Email = "johnDoe@gmail.com",
            SysAdmin = true,
            Password = "Test..123"
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(
            async () => await _auth0Service.UpdateUserAsync("userId", updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo("Email and Password cannot be changed at the same time."));
    }

    [Test]
    public async Task UpdateUserAsync_RoleUpdateEmail_SuccessfulUpdate()
    {
        var userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "JohnDoe@yahoo.com",
            Password = "",
            SysAdmin = true // Assuming this results in a role change to 'admin'
        };

        // Mock GetRole
        _mockAuth0Roles.Setup(service => service.GetRole(userId)).ReturnsAsync("admin");

        // Mock AssignRole
        _mockAuth0Roles.Setup(service => service.AssignRole("admin", userId)).Returns(Task.CompletedTask);

        // Mock UnassignRole
        _mockAuth0Roles.Setup(service => service.UnassignRoleAsync("client", userId)).Returns(Task.CompletedTask);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();

        // Prepare the mock user JSON response
        var mockUserJson = JsonConvert.SerializeObject(new Auth0UserResponse
        {
            UserId = "auth0|jahsjdkhasjkd",
            Name = "John Doe",
            FamilyName = "Doe",
            GivenName = "John",
            Email = "johnDoe@gmail.com",
            EmailVerified = false,
            Blocked = false,
            CreatedAt = "10/12/1222",
            UpdatedAt = "10/12/1222",
            Picture = "https://samplePicture.com"
        });

        var mockToken = new ManagementToken { Token = "mockToken" };
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Mock the response for GetUser
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockUserJson)
            });

        // Mock HttpClientFactory to return the HttpClient with the FakeHttpMessageHandler
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        // Mock UpdateUserDetailsInAuth0
        _mockAuth0Service
            .Setup(service => service.UpdateUserDetailsInAuth0(userId, It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _auth0Service.UpdateUserAsync(userId, updateUserDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.User.Name, Is.EqualTo("John Doe"));
            Assert.That(result.Role, Is.EqualTo("admin"));
        });
    }
    
     [Test]
    public async Task UpdateUserAsync_RoleUpdatePassword_SuccessfulUpdate()
    {
        var userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "",
            Password = "Test..123",
            SysAdmin = true // Assuming this results in a role change to 'admin'
        };

        // Mock GetRole
        _mockAuth0Roles.Setup(service => service.GetRole(userId)).ReturnsAsync("client");

        // Mock AssignRole
        _mockAuth0Roles.Setup(service => service.AssignRole("admin", userId)).Returns(Task.CompletedTask);

        // Mock UnassignRole
        _mockAuth0Roles.Setup(service => service.UnassignRoleAsync("client", userId)).Returns(Task.CompletedTask);

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();

        // Prepare the mock user JSON response
        var mockUserJson = JsonConvert.SerializeObject(new Auth0UserResponse
        {
            UserId = "auth0|jahsjdkhasjkd",
            Name = "John Doe",
            FamilyName = "Doe",
            GivenName = "John",
            Email = "johnDoe@gmail.com",
            EmailVerified = false,
            Blocked = false,
            CreatedAt = "10/12/1222",
            UpdatedAt = "10/12/1222",
            Picture = "https://samplePicture.com"
        });

        var mockToken = new ManagementToken { Token = "mockToken" };
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Mock the response for GetUser
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockUserJson)
            });

        // Mock HttpClientFactory to return the HttpClient with the FakeHttpMessageHandler
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        // Mock UpdateUserDetailsInAuth0
        _mockAuth0Service
            .Setup(service => service.UpdateUserDetailsInAuth0(userId, It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _auth0Service.UpdateUserAsync(userId, updateUserDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.User.Name, Is.EqualTo("John Doe"));
        });
    }
    
    
    [Test]
    public async Task UpdateUserAsync_RoleUpdateEmailAndPassword_Unsuccesfull()
    {
        var userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "JohnDoe@yahoo.com",
            Password = "Test..123",
            SysAdmin = true
        };
        var mockToken = new ManagementToken { Token = "mockToken" };
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);


        var exception = Assert.ThrowsAsync<ArgumentException>(
            async () => await _auth0Service.UpdateUserAsync("userId", updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo("Email and Password cannot be changed at the same time."));
    }
    
    [Test]
    public void UpdateUserAsync_UpdateFails_ThrowsUserUpdateException()
    {
        var userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "UpdatedFamilyName",
            GivenName = "UpdatedGivenName"
        };

        var mockToken = new ManagementToken { Token = "mockToken" };
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Prepare the fake HTTP handler
        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        var errorResponseContent = "Error updating user in Auth0";

        // Setup the mock HTTP response to simulate failure
        fakeHttpMessageHandler.SetupResponse($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}", 
            new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(errorResponseContent)
            });

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        // Act & Assert
        var exception = Assert.ThrowsAsync<Auth0Service.UserUpdateException>(
            async () => await _auth0Service.UpdateUserAsync(userId, updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo($"Error updating user in Auth0: {errorResponseContent}"));
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