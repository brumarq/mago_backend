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
    private readonly IAuth0RolesService _auth0RolesService;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public Auth0Service(ILogger<UserService> logger, IAuth0ManagementService auth0ManagementService,
        IHttpClientFactory httpClientFactory, IConfiguration configuration, IAuth0RolesService auth0RolesService)
    {
        _logger = logger;
        _auth0ManagementService = auth0ManagementService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _auth0RolesService = auth0RolesService;
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
        
        
        var roleName = createUserDto.SysAdmin ? "admin" : "client";
        await _auth0RolesService.AssignRole(roleName, auth0User.UserId);

        
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
            var result = await UpdateUserDetailsInAuth0(userId, updateDetails);
            if (!result)
            {
                throw new UserUpdateException("Something went wrong with updating the user");
            }
        }

        // Check and update the user role if needed
        var currentRole = await _auth0RolesService.GetRole(userId);
        var newRoleName = updateUserDto.SysAdmin ? "admin" : "client";

        if (newRoleName != currentRole)
        {
            if (!string.IsNullOrEmpty(currentRole))
            {
                await _auth0RolesService.UnassignRoleAsync(currentRole, userId);
            }
            await _auth0RolesService.AssignRole(newRoleName, userId);
        }

        return await GetUser(userId);
    }
    
    public async Task<bool> UpdateUserDetailsInAuth0(string userId, object userDetails)
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

        return true;
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
        var role = await _auth0RolesService.GetRole(userId);
        
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