using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using User = Domain.Entities.User;

namespace Application.Tests;

[TestFixture]
public class Auth0ServiceTests
{
    private Mock<ILogger<Auth0Service>> _mockLogger = null!;
    private Mock<IAuth0ManagementService> _mockAuth0ManagementService = null!;
    private Mock<IAuth0RolesService> _mockAuth0Roles = null!;
    private Mock<IHttpClientFactory> _mockHttpClientFactory = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Auth0Service _auth0Service = null!;
    private Mock<IAuth0Service> _mockAuth0Service = null!;
    private FakeHttpMessageHandler _fakeHttpMessageHandler = null!;
    private HttpClient _fakeHttpClient = null!;
    
    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<Auth0Service>>();
        _mockAuth0ManagementService = new Mock<IAuth0ManagementService>();
        _mockAuth0Service = new Mock<IAuth0Service>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockAuth0Roles = new Mock<IAuth0RolesService>();
        
        _mockConfiguration.Setup(config => config[It.IsAny<string>()]).Returns("configuredValue");

        _auth0Service = new Auth0Service(_mockLogger.Object, _mockAuth0ManagementService.Object, _mockHttpClientFactory.Object, _mockConfiguration.Object,_mockAuth0Roles.Object);
        
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
        
        // Mock the response for the user creation
        _fakeHttpMessageHandler.SetupResponse("https://random.com/api/v2/users", 
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
        _fakeHttpMessageHandler.SetupResponse("https://random.com/api/v2/roles/configuredValue/users", 
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(roleAssignmentResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
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
        Assert.ThrowsAsync<BadRequestException>(async () => await _auth0Service.CreateAuth0UserAsync(createUserDto));
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

        // Mock the failure response for the user creation
        var errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"Something\",\"errorCode\":\"inexistent_user\"}";
        _fakeHttpMessageHandler.SetupResponse("https://random.com/api/v2/users", 
            new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(errorResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);        
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
        var exception = Assert.ThrowsAsync<BadRequestException>(async () => await _auth0Service.CreateAuth0UserAsync(createUserDto));
        Assert.That(exception?.Message, Does.Contain("Something"));

        // Verify that the logger was called
        _mockLogger.Verify();
    }
    
    [Test]
    public void UpdateUserAsync_EmailAndPasswordSimultaneously_ThrowsArgumentException()
    {
        var updateUserDto = new UpdateUserDTO {
            FamilyName = "Doe",
            GivenName = "John",
            Email = "johnDoe@gmail.com",
            Password = "Test..123"
        };

        var exception = Assert.ThrowsAsync<BadRequestException>(
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
        };

        // Mock GetRole
        _mockAuth0Roles.Setup(service => service.GetRole(userId)).ReturnsAsync("admin");

        // Mock AssignRole
        _mockAuth0Roles.Setup(service => service.AssignRole("admin", userId)).Returns(Task.CompletedTask);

        // Mock UnassignRole
        _mockAuth0Roles.Setup(service => service.UnassignRoleAsync("client", userId)).Returns(Task.CompletedTask);
        
        // Prepare the mock user JSON response
        var mockUserJson = JsonConvert.SerializeObject(new User
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

        // Mock the response for GetUser
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockUserJson)
            });

        // Mock HttpClientFactory to return the HttpClient with the FakeHttpMessageHandler
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
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
        };

        // Mock GetRole
        _mockAuth0Roles.Setup(service => service.GetRole(userId)).ReturnsAsync("client");

        // Mock AssignRole
        _mockAuth0Roles.Setup(service => service.AssignRole("admin", userId)).Returns(Task.CompletedTask);

        // Mock UnassignRole
        _mockAuth0Roles.Setup(service => service.UnassignRoleAsync("client", userId)).Returns(Task.CompletedTask);
        
        // Prepare the mock user JSON response
        var mockUserJson = JsonConvert.SerializeObject(new User
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

        // Mock the response for GetUser
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockUserJson)
            });

        // Mock HttpClientFactory to return the HttpClient with the FakeHttpMessageHandler
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
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
    public void UpdateUserAsync_RoleUpdateEmailAndPassword_Unsuccesfull()
    {
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "JohnDoe@yahoo.com",
            Password = "Test..123",
        };
        
        var exception = Assert.ThrowsAsync<BadRequestException>(
            () => _auth0Service.UpdateUserAsync("userId", updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo("Email and Password cannot be changed at the same time."));
    }
    
    [Test]
    public void UpdateUserAsync_UpdateFails_ThrowsUserUpdateException()
    {
        const string userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "UpdatedFamilyName",
            GivenName = "UpdatedGivenName"
        };

        // Prepare the fake HTTP handler
        var errorResponseContent = "{\"statusCode\":403,\"error\":\"Not Found\",\"message\":\"Bad request\",\"errorCode\":\"inexistent_user\"}";


        // Setup the mock HTTP response to simulate failure
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}", 
            new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(errorResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        // Act & Assert
        var exception = Assert.ThrowsAsync<BadRequestException>(
            async () => await _auth0Service.UpdateUserAsync(userId, updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo("Bad request"));
    }
    
    
    [Test]
    public void GetUser_UnsuccessfulResponse_ThrowsUserNotFoundException()
    {
        const string userId = "someUserId";
        var errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"User not found\",\"errorCode\":\"inexistent_user\"}";

        var mockToken = new ManagementToken { Token = "mockToken" };

        // Mock GetToken to return a mock token
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Setup the fake HTTP response for an error scenario
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.NotFound) // You can use other error codes as needed
            {
                Content = new StringContent(errorResponseContent)
            });

        // Mock HttpClientFactory to return the HttpClient with the FakeHttpMessageHandler
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        // Act and Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _auth0Service.GetUser(userId));

        // Assert that the exception message is as expected
        Assert.That(ex?.Message, Is.EqualTo("User not found"));
    }

    
    [Test]
    public async Task GetUsersByRoleId_SuccessfulResponse_ReturnsUsers()
    {
        const string roleId = "testRoleId";
        var mockToken = new ManagementToken { Token = "mockToken" };
        var mockUsers = new List<UserCompressed> { /* populate with test data */ };
        var mockResponseContent = new Auth0UsersResponse
        {
            Users = mockUsers,
            Total = mockUsers.Count
        };

        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{roleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockResponseContent), Encoding.UTF8, "application/json")
            });

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        var result = await _auth0Service.GetUsersByRoleId(roleId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockUsers.Count));
    }

    
    [Test]
    public void GetUsersByRoleId_UnsuccessfulResponse_ThrowsException()
    {
        const string roleId = "testRoleId";
        var mockToken = new ManagementToken { Token = "mockToken" };
        var errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"User not found\",\"errorCode\":\"inexistent_user\"}";

        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{roleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorResponseContent, Encoding.UTF8, "application/json")
            });

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _auth0Service.GetUsersByRoleId(roleId));

        Assert.That(ex?.Message, Is.EqualTo("User not found"));
    }

    
    [Test]
    public async Task DeleteUserAsync_SuccessfulDeletion_ReturnsTrue()
    {
        var userId = "existingUserId";
        
        // Mock the response for user retrieval
        var userJson = JsonConvert.SerializeObject(new User
        {
            // Initialize the User properties as expected by the UserDTO
        });
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userJson, Encoding.UTF8, "application/json")
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        var result = await _auth0Service.DeleteUserAsync(userId);

        Assert.IsTrue(result);
    }

    [Test]
    public void DeleteUserAsync_UnsuccessfulResponse_ThrowsException()
    {
        const string userId = "nonExistingUserId";
        var errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"User not found\",\"errorCode\":\"inexistent_user\"}";

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _auth0Service.DeleteUserAsync(userId));
        Assert.That(ex?.Message, Is.EqualTo("User not found"));
    }
    
    [Test]
    public async Task GetAllUsers_SuccessfullyReturnsCombinedUsers()
    {
        const string adminRoleId = "adminRoleId"; // Mock role ID for admin
        const string clientRoleId = "clientRoleId"; // Mock role ID for client
        var mockAdminUsers = new List<UserCompressed> { /* Mock data for admin users */ };
        var mockClientUsers = new List<UserCompressed> { /* Mock data for client users */ };
        var mockToken = new ManagementToken { Token = "mockToken" };

        // Setup configuration for role IDs
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns(adminRoleId);
        _mockConfiguration.Setup(c => c["Auth0-Roles:client"]).Returns(clientRoleId);

        // Mock GetToken
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Mock GetUsersByRoleId for each role, assuming one page per role for simplicity
        var mockAdminResponseContent = new Auth0UsersResponse
        {
            Users = mockAdminUsers,
            Total = mockAdminUsers.Count
        };
        var mockClientResponseContent = new Auth0UsersResponse
        {
            Users = mockClientUsers,
            Total = mockClientUsers.Count
        };

        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{adminRoleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockAdminResponseContent), Encoding.UTF8, "application/json")
            });

        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{clientRoleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockClientResponseContent), Encoding.UTF8, "application/json")
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act
        var result = await _auth0Service.GetAllUsers();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockAdminUsers.Count + mockClientUsers.Count));
    }

    
    [Test]
    public async Task GetAllUsers_MissingRoleIdInConfiguration_SkipsRole()
    {
        const string clientRoleId = "clientRoleId";
        var mockClientUsers = new List<UserCompressed> { /* Mock data for client users */ };
        var mockToken = new ManagementToken { Token = "mockToken" };

        // Setup configuration with a missing role ID for admin
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns("");
        _mockConfiguration.Setup(c => c["Auth0-Roles:client"]).Returns(clientRoleId);

        // Mock GetToken
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);

        // Mock GetUsersByRoleId for client role only
        // Assuming you have one page of data
        var mockResponseContent = new Auth0UsersResponse
        {
            Users = mockClientUsers,
            Total = mockClientUsers.Count
        };

        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{clientRoleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockResponseContent), Encoding.UTF8, "application/json")
            });
    
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act
        var result = await _auth0Service.GetAllUsers();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockClientUsers.Count)); // Should only return client users
    }

    
    [Test]
    public void GetAllUsers_UnsuccessfulApiResponse_ThrowsException()
    {
        const string adminRoleId = "adminRoleId";
        var mockToken = new ManagementToken { Token = "mockToken" };

        // Setup configuration for role IDs
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns(adminRoleId);

        // Mock GetToken
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(mockToken);
        var errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"Server error\",\"errorCode\":\"inexistent_user\"}";

        // Mock an unsuccessful GetUsersByRoleId response for the first page
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{adminRoleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(errorResponseContent, Encoding.UTF8, "application/json")
            });
    
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert
        var ex = Assert.ThrowsAsync<CustomException>(async () => await _auth0Service.GetAllUsers());
        Assert.That(ex?.Message, Is.EqualTo("Server error"));
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
