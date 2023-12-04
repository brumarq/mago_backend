using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ApplicationServices;

public class Auth0Service : IAuth0Service
{
    private readonly ILogger<UserService> _logger;
    private readonly IAuth0ManagementService _auth0ManagementService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public Auth0Service(ILogger<UserService> logger, IAuth0ManagementService auth0ManagementService,
        IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _auth0ManagementService = auth0ManagementService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Create User
    public async Task<Auth0UserResponseDto> CreateAuth0UserAsync(CreateUserDTO createUserDto)
    {

        ValidateEmail(createUserDto.Email);
        ValidatePasswordStrength(createUserDto.Password);

        if (string.IsNullOrWhiteSpace(createUserDto.FamilyName) || string.IsNullOrWhiteSpace(createUserDto.GivenName))
        {
            throw new InvalidUserDetailsException("Name cannot be empty or whitespace.");
        }
        
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to create a new user in Auth0
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users")
        {
            Content = JsonContent.Create(new
            {
                email = createUserDto.Email,
                family_name = createUserDto.FamilyName,
                given_name = createUserDto.GivenName,
                name = createUserDto.GivenName + " " + createUserDto.FamilyName,
                password = createUserDto.Password,
                connection = "Username-Password-Authentication"
            }),
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error creating user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new UserCreationException($"Error creating user in Auth0: {errorContent}");
        }

        var auth0User = await response.Content.ReadFromJsonAsync<Auth0UserResponse>();
        
        if (auth0User?.UserId == null)
        {
            throw new Exception();
        }
        
        var roleName = createUserDto.SysAdmin ? "admin" : "client";
        await AssignRole(roleName, auth0User.UserId);

        
        return new Auth0UserResponseDto
        {
            User = auth0User,
            Role = roleName
        };
    }
    
    // Update User
    public async Task<Auth0UserResponseDto> UpdateUserAsync(string userId, UpdateUserDTO updateUserDto)
    {
        // Check if both Email and Password are being updated simultaneously
        if (!string.IsNullOrWhiteSpace(updateUserDto.Email) && !string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            throw new ArgumentException("Email and Password cannot be changed at the same time.");
        }

        var updateDetails = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(updateUserDto.FamilyName))
        {
            updateDetails.Add("family_name", updateUserDto.FamilyName);
        }
        if (!string.IsNullOrWhiteSpace(updateUserDto.GivenName))
        {
            updateDetails.Add("given_name", updateUserDto.GivenName);
        }
        if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
        {
            ValidateEmail(updateUserDto.Email); // Validate email
            updateDetails.Add("email", updateUserDto.Email);
        }
        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            ValidatePasswordStrength(updateUserDto.Password); // Validate password strength
            updateDetails.Add("password", updateUserDto.Password);
        }

        if (updateDetails.Count > 0)
        {
            await UpdateUserDetailsInAuth0(userId, updateDetails);
        }

        // Check and update the user role if needed
        var currentRole = await GetRole(userId);
        var newRoleName = updateUserDto.SysAdmin ? "admin" : "client";

        if (newRoleName != currentRole)
        {
            if (!string.IsNullOrEmpty(currentRole))
            {
                await UnassignRoleAsync(currentRole, userId);
            }
            await AssignRole(newRoleName, userId);
        }

        return await GetUser(userId);
    }
    
    private async Task UpdateUserDetailsInAuth0(string userId, object userDetails)
    {
        var token = await _auth0ManagementService.GetToken();
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Patch, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}")
        {
            Content = JsonContent.Create(userDetails),
            Headers = { { "Authorization", $"Bearer {token.Token}" } }
        };

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error updating user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new UserUpdateException($"Error updating user in Auth0: {errorContent}");
        }
    }
    
    // Get User/s
    public async Task<Auth0UserResponseDto> GetUser(string userId)
    {
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to assign a role to a user
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error retrieving user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new UserNotFoundException($"Error retrieving user in Auth0: {errorContent}");
        }

        var auth0User = await response.Content.ReadFromJsonAsync<Auth0UserResponse>();
        var role = await GetRole(userId);
        
        if (auth0User == null)
        {
            throw new Exception();
        }

        return new Auth0UserResponseDto
        {
            User = auth0User,
            Role = role
        };
    }
    
    public async Task<List<Auth0UserResponseDto>> GetAllUsers()
    {
        var adminUsers = await GetUsersByRoleName("admin");
        var clientUsers = await GetUsersByRoleName("client");

        // Combine the lists
        var allUsersWithRoles = new List<Auth0UserResponseDto>();
        allUsersWithRoles.AddRange(adminUsers);
        allUsersWithRoles.AddRange(clientUsers);

        return allUsersWithRoles;
    }

    private async Task<List<Auth0UserResponseDto>> GetUsersByRoleName(string roleName)
    {
        var roleId = _configuration[$"Auth0-Roles:{roleName}"];
        if (string.IsNullOrEmpty(roleId))
        {
            _logger.LogError($"Role ID for {roleName} not found in configuration.");
            return new List<Auth0UserResponseDto>();
        }

        var users = await GetUsersByRoleId(roleId);
        return users.Select(user => new Auth0UserResponseDto
        {
            User = user,
            Role = roleName
        }).ToList();
    }

    private async Task<List<Auth0UserResponse>> GetUsersByRoleId(string roleId)
    {
        var token = await _auth0ManagementService.GetToken();

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/roles/{roleId}/users")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error retrieving users for role {RoleId} in Auth0. Status Code: {StatusCode}, Details: {Details}", roleId, response.StatusCode, errorContent);
            throw new Exception($"Error retrieving users for role {roleId} in Auth0: {errorContent}");
        }

        var users = await response.Content.ReadFromJsonAsync<List<Auth0UserResponse>>();
        return users ?? new List<Auth0UserResponse>();
    }

    // Delete User
    
    public async Task<bool> DeleteUserAsync(string userId)
    {
        // First, check if the user exists
        var existingUser = await GetUser(userId);
        if (existingUser == null)
        {
            throw new ArgumentException($"User with ID {userId} does not exist.");
        }

        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to delete the user
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error deleting user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new Exception($"Error deleting user in Auth0: {errorContent}");
        }

        return true;
    }

    
    /* ---------
        Roles
    --------- */
    
    private async Task UnassignRoleAsync(string roleName, string userId)
    {
        var token = await _auth0ManagementService.GetToken();
        var roleId = _configuration[$"Auth0-Roles:{roleName}"];

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}/roles")
        {
            Content = JsonContent.Create(new { roles = new[] { roleId } }),
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error unassigning role from user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new UserRoleException($"Error unassigning role from user in Auth0: {errorContent}");
        }
    }

    
    private async Task AssignRole(string roleName, string userId)
    {
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/roles/{_configuration[$"Auth0-Roles:{roleName}"]}/users")
        {
            Content = JsonContent.Create(new
            {
                users = new[] {$"{userId}"}
            }),
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error assigning role to user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new UserRoleException($"Error assigning role to user in Auth0: {errorContent}");
        }

        var auth0User = await response.Content.ReadFromJsonAsync<Auth0UserResponse>();

        if (auth0User == null)
        {
            throw new Exception();
        }
    }
    
    private async Task<string> GetRole(string userId)
    {
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{userId}/roles")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error retrieving roles for user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new UserRoleException($"Error retrieving roles for user in Auth0: {errorContent}");
        }

        // Read the response content and deserialize it
        var roles = await response.Content.ReadFromJsonAsync<List<Role>>();

        // Check if the roles list is not empty, then return the name of the first role
        if (roles != null && roles.Any())
        {
            return roles.First().Name;
        }

        // If the roles list is empty, return an empty string or null
        return "";
    }
    
    /* --------------------
        Validation Methods
    -------------------- */
    private void ValidatePasswordStrength(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidUserDetailsException("Password cannot be empty.");
        }

        var minLength = 8;
        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (password.Length < minLength)
        {
            throw new InvalidUserDetailsException($"Password must be at least {minLength} characters long.");
        }
        if (!hasUpper)
        {
            throw new InvalidUserDetailsException("Password must contain at least one uppercase letter.");
        }
        if (!hasLower)
        {
            throw new InvalidUserDetailsException("Password must contain at least one lowercase letter.");
        }
        if (!hasDigit)
        {
            throw new InvalidUserDetailsException("Password must contain at least one digit.");
        }
        if (!hasSpecial)
        {
            throw new InvalidUserDetailsException("Password must contain at least one special character.");
        }
    }
    
    private static void ValidateEmail(string? email)
    {
        try
        {
            if (email == null) throw new InvalidUserDetailsException("Email cannot be empty.");;
            
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
            {
                throw new InvalidUserDetailsException("Invalid email format.");
            }
        }
        catch
        {
            throw new InvalidUserDetailsException("Invalid email format.");
        }
    }
    
    
    // Exception
    
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) {}
    }

    public class UserCreationException : Exception
    {
        public UserCreationException(string message) : base(message) {}
    }

    public class UserUpdateException : Exception
    {
        public UserUpdateException(string message) : base(message) {}
    }

    public class UserRoleException : Exception
    {
        public UserRoleException(string message) : base(message) {}
    }

    public class InvalidUserDetailsException : Exception
    {
        public InvalidUserDetailsException(string message) : base(message) {}
    }

}