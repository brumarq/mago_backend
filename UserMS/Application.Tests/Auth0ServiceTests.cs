using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Enums;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using User = Domain.Entities.User;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class Auth0ServiceTests
{
    // Mock objects for dependencies and services
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
        // Initialize mock objects for each dependency
        _mockLogger = new Mock<ILogger<Auth0Service>>();
        _mockAuth0ManagementService = new Mock<IAuth0ManagementService>();
        _mockAuth0Service = new Mock<IAuth0Service>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockAuth0Roles = new Mock<IAuth0RolesService>();
        
        // Setup configuration with predetermined values for testing
        _mockConfiguration.Setup(config => config[It.IsAny<string>()]).Returns("configuredValue");

        // Initialize the service under test with mock dependencies
        _auth0Service = new Auth0Service(_mockLogger.Object, _mockAuth0ManagementService.Object, _mockHttpClientFactory.Object, _mockConfiguration.Object, _mockAuth0Roles.Object);
        
        // Additional configuration setups for role IDs and management audience
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns("adminRoleId");
        _mockConfiguration.Setup(c => c["Auth0-Roles:client"]).Returns("clientRoleId");
        _mockConfiguration.Setup(c => c["Auth0-Management:Audience"]).Returns("https://random.com/api/v2/");
        
        // Mock the Auth0 Management Service's token retrieval
        _mockAuth0ManagementService.Setup(service => service.GetToken()).ReturnsAsync(new ManagementToken { Token = "mockToken" });
        
        // Initialize fake HTTP handler and client for mocking HTTP requests
        _fakeHttpMessageHandler = new FakeHttpMessageHandler();
        _fakeHttpClient = new HttpClient(_fakeHttpMessageHandler);
    }
    
    [TearDown]
    public void TearDown()
    {
        // Dispose the fake HTTP handler and client after each test
        _fakeHttpMessageHandler.Dispose();
        _fakeHttpClient.Dispose();
    }
    
    /*######/
    /* Create User Tests
    /#######*/
    
    [Test]
    public async Task CreateAuth0UserAsync_SuccessfulCreation_ReturnsUser()
    {
        // Arrange: Set up a new user DTO and mock response for successful user creation
        var createUserDto = new CreateUserDTO {
            Email = "johnDoe@gmail.com",
            FamilyName = "Doe",
            GivenName = "John",
            Password = "StrongPass!23",
            SysAdmin = false
        };
        
        // Mock the HTTP response for user creation with a successful result
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

        // Mock the response for the role assignment
        var roleAssignmentResponseContent = JsonConvert.SerializeObject(new { result = "success" });
        _fakeHttpMessageHandler.SetupResponse("https://random.com/api/v2/roles/configuredValue/users", 
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(roleAssignmentResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act: Attempt to create a new Auth0 user
        var result = await _auth0Service.CreateAuth0UserAsync(createUserDto);

        // Assert: Verify the user is created successfully and the returned user matches expected values
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.User?.Name, Is.EqualTo("John Doe"));
            Assert.That(result.Role, Is.EqualTo("client"));
        });
    }
    
    [Test]
    public void CreateAuth0UserAsync_WrongGivenName_ThrowsInvalidUserDetailsException()
    {
        // Arrange: Set up a new user DTO with invalid details (empty GivenName and FamilyName)
        var createUserDto = new CreateUserDTO {
            Email = "johnDoe@gmail.com",
            FamilyName = "",
            GivenName = "",
            Password = "StrongPass!23",
            SysAdmin = false
        };
    
        // Act & Assert: Expect BadRequestException to be thrown due to invalid user details
        Assert.ThrowsAsync<BadRequestException>(async () => await _auth0Service.CreateAuth0UserAsync(createUserDto));
    }
    
    [Test]
    public void CreateAuth0UserAsync_Auth0UserCreationFails_ThrowsUserCreationException()
    {
        // Arrange: Set up a new user DTO and mock an unsuccessful response for user creation
        var createUserDto = new CreateUserDTO {
            Email = "test@example.com",
            FamilyName = "Test",
            GivenName = "User",
            Password = "StrongPass!23",
            SysAdmin = false
        };
        
        const string errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"Something\",\"errorCode\":\"inexistent_user\"}";
        
        // Mock the failure response for the user creation
        _fakeHttpMessageHandler.SetupResponse("https://random.com/api/v2/users", 
            new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(errorResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Expect BadRequestException to be thrown due to a failure in user creation
        var exception = Assert.ThrowsAsync<BadRequestException>(async () => await _auth0Service.CreateAuth0UserAsync(createUserDto));
        Assert.That(exception?.Message, Does.Contain("Something"));

        // Verify that the logger was called, indicating logging of the exception
        _mockLogger.Verify();
    }
    
    /*######/
    /* Update User Tests
    /#######*/
    
    [Test]
    public void UpdateUserAsync_EmailAndPasswordSimultaneously_ThrowsArgumentException()
    {
        // Arrange: Create a DTO with both email and password to simulate an invalid update scenario
        var updateUserDto = new UpdateUserDTO {
            FamilyName = "Doe",
            GivenName = "John",
            Email = "johnDoe@gmail.com",
            Password = "Test..123"
        };

        // Act & Assert: Expect a BadRequestException when trying to update both email and password simultaneously
        var exception = Assert.ThrowsAsync<BadRequestException>(
            async () => await _auth0Service.UpdateUserAsync("userId", updateUserDto)
        );
        
        Assert.That(exception?.Message, Is.EqualTo("Email and Password cannot be changed at the same time."));
    }

    [Test]
    public async Task UpdateUserAsync_RoleUpdateEmail_SuccessfulUpdate()
    {
        // Arrange: Set up the user details for the update, including email but no password
        const string userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "JohnDoe@yahoo.com",
            Password = "",
        };

        // Mocking dependencies for role retrieval and assignment
        _mockAuth0Roles.Setup(service => service.GetRole(userId)).ReturnsAsync("admin");

        _mockAuth0Roles.Setup(service => service.AssignRole("admin", userId)).Returns(Task.CompletedTask);

        _mockAuth0Roles.Setup(service => service.UnassignRoleAsync("client", userId)).Returns(Task.CompletedTask);
        
        // Mock the GetUser response and HttpClientFactory
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

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockUserJson)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        _mockAuth0Service
            .Setup(service => service.UpdateUserDetailsInAuth0(userId, It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(true);

        
        // Act: Attempt to update the user's email and role
        var result = await _auth0Service.UpdateUserAsync(userId, updateUserDto);

        // Assert: Verify that the update was successful and the returned user matches expected values
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.User?.Name, Is.EqualTo("John Doe"));
            Assert.That(result.Role, Is.EqualTo("admin"));
        });
    }
    
     [Test]
    public async Task UpdateUserAsync_RoleUpdatePassword_SuccessfulUpdate()
    {
        // Arrange: Set up the user details for the update, including password but no email
        const string userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "",
            Password = "Test..123",
        };

        // Mocking dependencies for role retrieval and assignment
        _mockAuth0Roles.Setup(service => service.GetRole(userId)).ReturnsAsync("client");

        _mockAuth0Roles.Setup(service => service.AssignRole("admin", userId)).Returns(Task.CompletedTask);

        _mockAuth0Roles.Setup(service => service.UnassignRoleAsync("client", userId)).Returns(Task.CompletedTask);
        
        // Mock the GetUser response and HttpClientFactory
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

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockUserJson)
            });

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        _mockAuth0Service
            .Setup(service => service.UpdateUserDetailsInAuth0(userId, It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(true);

        // Act: Attempt to update the user's password and role
        var result = await _auth0Service.UpdateUserAsync(userId, updateUserDto);

        // Assert: Verify that the update was successful and the returned user matches expected values
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.User?.Name, Is.EqualTo("John Doe"));
        });
    }
    
    
    [Test]
    public void UpdateUserAsync_RoleUpdateEmailAndPassword_Unsuccesfull()
    {
        // Arrange: Set up a user DTO with both email and password for an invalid update scenario
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "Three",
            GivenName = "John",
            Email = "JohnDoe@yahoo.com",
            Password = "Test..123",
        };
        
        // Act & Assert: Expect BadRequestException due to simultaneous email and password update attempt
        var exception = Assert.ThrowsAsync<BadRequestException>(
            () => _auth0Service.UpdateUserAsync("userId", updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo("Email and Password cannot be changed at the same time."));
    }
    
    [Test]
    public void UpdateUserAsync_UpdateFails_ThrowsUserUpdateException()
    {
        // Arrange: Set up a user DTO and mock a failure response for user update
        const string userId = "userId";
        var updateUserDto = new UpdateUserDTO
        {
            FamilyName = "UpdatedFamilyName",
            GivenName = "UpdatedGivenName"
        };

        const string errorResponseContent = "{\"statusCode\":403,\"error\":\"Not Found\",\"message\":\"Bad request\",\"errorCode\":\"inexistent_user\"}";

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}", 
            new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(errorResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act & Assert: Expect BadRequestException due to an unsuccessful update operation
        var exception = Assert.ThrowsAsync<BadRequestException>(
            async () => await _auth0Service.UpdateUserAsync(userId, updateUserDto)
        );
        Assert.That(exception?.Message, Is.EqualTo("Bad request"));
    }
    
    /*######/
    /* Get User Tests
    /#######*/
    
    [Test]
    public void GetUser_UnsuccessfulResponse_ThrowsUserNotFoundException()
    {
        // Arrange: Set up mock response for failed user retrieval
        const string userId = "someUserId";
        const string errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"User not found\",\"errorCode\":\"inexistent_user\"}";

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.NotFound) // You can use other error codes as needed
            {
                Content = new StringContent(errorResponseContent)
            });

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act & Assert: Expect NotFoundException when user retrieval fails
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _auth0Service.GetUser(userId));
        Assert.That(ex?.Message, Is.EqualTo("User not found"));
    }

    /*######/
    /* Get User by Role Tests
    /#######*/
    
    [Test]
    public async Task GetUsersByRoleId_SuccessfulResponse_ReturnsUsers()
    {
        // Arrange: Set up mock users and response for successful user retrieval by role ID
        const string roleId = "testRoleId";
        var mockUsers = new List<UserCompressed>
        {
            new()
            {
                UserId = "auth0|659ac2c3bba7718e2eae6647",
                Name = "brunocm@pm.me",
                Email = "brunocm@pm.me",
                Picture = "https://avatar.com"
            },
            new()
            {
                UserId = "auth0|1234567890abcdef12345678",
                Name = "johndoe@example.com",
                Email = "johndoe@example.com",
                Picture = "https://avatar.com"
            }
        };        
        
        var mockResponseContent = new Auth0UsersResponse
        {
            Users = mockUsers,
            Total = mockUsers.Count
        };

        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{roleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockResponseContent), Encoding.UTF8, "application/json")
            });

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act: Retrieve users by role ID
        var result = await _auth0Service.GetUsersByRoleId(roleId, 1, 100);

        // Assert: Verify the result is as expected
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockUsers.Count));
        for (var i = 0; i < mockUsers.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result[i].Name, Is.EqualTo(mockUsers[i].Name));
                Assert.That(result[i].Email, Is.EqualTo(mockUsers[i].Email));
                Assert.That(result[i].Picture, Is.EqualTo(mockUsers[i].Picture));
            });
        }
    }
    
    [Test]
    public void GetUsersByRoleId_UnsuccessfulResponse_ThrowsException()
    {
        // Arrange: Set up mock response for failed user retrieval by role ID
        const string roleId = "testRoleId";
        const string errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"User not found\",\"errorCode\":\"inexistent_user\"}";

        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/{roleId}/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorResponseContent, Encoding.UTF8, "application/json")
            });

        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act & Assert: Expect NotFoundException when retrieval by role ID fails
        var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _auth0Service.GetUsersByRoleId(roleId, 1, 100));
        Assert.That(ex?.Message, Is.EqualTo("User not found"));
    }
    
    /*######/
    /* Get All Users Tests
    /#######*/
    
    [Test]
    public async Task GetAllUsers_SuccessfullyReturnsCombinedUsers()
    {
        // Arrange: Create mock admin and client users
        var mockAdminUsers = new List<UserCompressed> {
            new()
            {
                UserId = "auth0|659ac2c3bba7718e2eae6647",
                Name = "brunocm@pm.me",
                Email = "brunocm@pm.me",
                Picture = "https://avatar.com"
            },
            new()
            {
                UserId = "auth0|1234567890abcdef12345678",
                Name = "johndoe@example.com",
                Email = "johndoe@example.com",
                Picture = "https://avatar.com"
            }
        };
        var mockClientUsers = new List<UserCompressed> {
            new()
            {
                UserId = "auth0|659ac2c3bba7718e2ea2345",
                Name = "client1",
                Email = "client1@pm.me",
                Picture = "https://avatar.com"
            },
            new()
            {
                UserId = "auth0|1234567890abcdef123534534",
                Name = "client2",
                Email = "client2@example.com",
                Picture = "https://avatar.com"
            }
        };

        // Prepare mock responses for admin and client user retrieval
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

        // Mock HTTP responses for both admin and client user lists
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/adminRoleId/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockAdminResponseContent), Encoding.UTF8, "application/json")
            });

        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/clientRoleId/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockClientResponseContent), Encoding.UTF8, "application/json")
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act: Call the method to test
        var result = await _auth0Service.GetAllUsers();

        // Assert: Check if the result is not null and has the expected user count
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockAdminUsers.Count + mockClientUsers.Count));

        // Assert: Verify each user's properties in the result
        var allMockUsers = mockAdminUsers.Concat(mockClientUsers).ToList();
        for (var i = 0; i < allMockUsers.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result[i].User.Name, Is.EqualTo(allMockUsers[i].Name), $"Name mismatch at index {i}");
                Assert.That(result[i].User.Email, Is.EqualTo(allMockUsers[i].Email), $"Email mismatch at index {i}");
                Assert.That(result[i].User.Picture, Is.EqualTo(allMockUsers[i].Picture), $"Picture mismatch at index {i}");
            });
        }
    }

    
    [Test]
    public async Task GetAllUsers_SuccessfullyReturnsAdminUsers()
    {
        // Arrange: Create a list of mock admin users
        var mockAdminUsers = new List<UserCompressed> {
            new()
            {
                UserId = "auth0|659ac2c3bba7718e2eae6647",
                Name = "brunocm@pm.me",
                Email = "brunocm@pm.me",
                Picture = "https://avatar.com"
            },
            new()
            {
                UserId = "auth0|1234567890abcdef12345678",
                Name = "johndoe@example.com",
                Email = "johndoe@example.com",
                Picture = "https://avatar.com"
            }
        };

        var mockAdminResponseContent = new Auth0UsersResponse
        {
            Users = mockAdminUsers,
            Total = mockAdminUsers.Count
        };

        // Mock the response for admin user retrieval
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/adminRoleId/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockAdminResponseContent), Encoding.UTF8, "application/json")
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act: Call the method to test
        var result = await _auth0Service.GetAllUsers(1, 100, UserRole.Admin);

        // Assert: Check the result is not null and has the expected number of users
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockAdminUsers.Count));

        // Check each user in the result for correct properties
        for (var i = 0; i < mockAdminUsers.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result[i].User.Name, Is.EqualTo(mockAdminUsers[i].Name), $"Name mismatch at index {i}");
                Assert.That(result[i].User.Email, Is.EqualTo(mockAdminUsers[i].Email), $"Email mismatch at index {i}");
                Assert.That(result[i].User.Picture, Is.EqualTo(mockAdminUsers[i].Picture), $"Picture mismatch at index {i}");
            });
        }
    }

    
    [Test]
    public async Task GetAllUsers_MissingRoleIdInConfiguration_SkipsRole()
    {
        // Arrange: Create a list of mock client users
        var mockClientUsers = new List<UserCompressed> {
            new()
            {
                UserId = "auth0|659ac2c3bba7718e2ea2345",
                Name = "client1",
                Email = "client1@pm.me",
                Picture = "https://avatar.com"
            },
            new()
            {
                UserId = "auth0|1234567890abcdef123534534",
                Name = "client2",
                Email = "client2@example.com",
                Picture = "https://avatar.com"
            }
        }; 
        
        var mockResponseContent = new Auth0UsersResponse
        {
            Users = mockClientUsers,
            Total = mockClientUsers.Count
        };
        
        // Setup configuration with a missing role ID for admin
        _mockConfiguration.Setup(c => c["Auth0-Roles:admin"]).Returns("");

        // Mock the response for client user retrieval
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/clientRoleId/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockResponseContent), Encoding.UTF8, "application/json")
            });
    
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act: Call the method to test
        var result = await _auth0Service.GetAllUsers();

        // Assert: Verify the result is not null and has the correct user count
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(mockClientUsers.Count));

        // Check each user in the result for correct properties
        for (var i = 0; i < mockClientUsers.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result[i].User.Name, Is.EqualTo(mockClientUsers[i].Name), $"Name mismatch at index {i}");
                Assert.That(result[i].User.Email, Is.EqualTo(mockClientUsers[i].Email), $"Email mismatch at index {i}");
                Assert.That(result[i].User.Picture, Is.EqualTo(mockClientUsers[i].Picture), $"Picture mismatch at index {i}");
            });
        }
    }

    
    [Test]
    public void GetAllUsers_UnsuccessfulApiResponse_ThrowsException()
    {
        // Arrange: Set up a mock error response
        const string errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"Server error\",\"errorCode\":\"inexistent_user\"}";

        // Mock an unsuccessful GetUsersByRoleId response for the first page
        _fakeHttpMessageHandler.SetupResponse(
            $"https://random.com/api/v2/roles/adminRoleId/users?per_page=100&page=0&include_totals=true",
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(errorResponseContent, Encoding.UTF8, "application/json")
            });
    
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Expect a CustomException to be thrown with a specific error message
        var ex = Assert.ThrowsAsync<CustomException>(async () => await _auth0Service.GetAllUsers());
        Assert.That(ex?.Message, Is.EqualTo("Server error"));
    }
    
    /*######/
    /* Delete User Tests
    /#######*/
    
    [Test]
    public async Task DeleteUserAsync_SuccessfulDeletion_ReturnsTrue()
    {
        // Arrange: Set up a mock user ID and mock response for successful user deletion
        const string userId = "auth0|659ac2c3bba7718e2eae6647";
        
        var userJson = JsonConvert.SerializeObject(new User
        {
            UserId = "auth0|659ac2c3bba7718e2eae6647",
            Name = "brunocm@pm.me",
            Email = "brunocm@pm.me",
            Picture = "https://avatar.com"
        });
        
        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userJson, Encoding.UTF8, "application/json")
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act: Call the method to test user deletion
        var result = await _auth0Service.DeleteUserAsync(userId);
        
        // Assert: Verify that the method returns true indicating successful deletion
        Assert.That(result, Is.True);
    }

    [Test]
    public void DeleteUserAsync_UnsuccessfulResponse_ThrowsException()
    {
        // Arrange: Set up a mock user ID and mock response for a failed user deletion attempt
        const string userId = "nonExistingUserId";
        var errorResponseContent = "{\"statusCode\":404,\"error\":\"Not Found\",\"message\":\"User not found\",\"errorCode\":\"inexistent_user\"}";

        _fakeHttpMessageHandler.SetupResponse($"https://random.com/api/v2/users/{userId}",
            new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorResponseContent)
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);
        
        // Act & Assert: Expect a NotFoundException to be thrown with a specific error message
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _auth0Service.DeleteUserAsync(userId));
        Assert.That(ex?.Message, Is.EqualTo("User not found"));
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
