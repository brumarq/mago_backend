using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ApplicationServices;

public class Auth0Service: IAuth0Service
{
    private readonly ILogger<UserService> _logger;
    private readonly IAuth0ManagementService _auth0ManagementService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public Auth0Service(ILogger<UserService> logger, IAuth0ManagementService auth0ManagementService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _auth0ManagementService = auth0ManagementService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    
    public async Task<Auth0UserResponseDto> CreateAuth0UserAsync(CreateUserDTO createUserDto)
    {
        
        ValidateEmail(createUserDto.Email);
        ValidatePasswordStrength(createUserDto.Password);
        
        if (string.IsNullOrWhiteSpace(createUserDto.Name))
        {
            throw new ArgumentException("Name cannot be empty or whitespace.");
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
                name = createUserDto.Name,
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
            throw new Exception($"Error creating user in Auth0: {errorContent}");
        }

        var auth0User = await response.Content.ReadFromJsonAsync<Auth0UserResponse>();
        
        if (auth0User?.UserId == null)
        {
            throw new Exception();
        }
        
        var roleName = createUserDto.SysAdmin ? "admin" : "client";
        await AddRole(roleName, auth0User.UserId);

        
        return new Auth0UserResponseDto
        {
            User = auth0User,
            Role = roleName
        };
    }
    
    private async Task AddRole(string roleName, string userId)
    {
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to assign a role to a user
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
            throw new Exception($"Error assigning role to user in Auth0: {errorContent}");
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

        // Prepare the request to get roles of a user
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
            throw new Exception($"Error retrieving roles for user in Auth0: {errorContent}");
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
            _logger.LogError("Error assigning role to user in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new Exception($"Error assigning role to user in Auth0: {errorContent}");
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
    
    
    // Validation Methods
    private void ValidatePasswordStrength(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.");
        }

        var minLength = 8;
        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (password.Length < minLength)
        {
            throw new ArgumentException($"Password must be at least {minLength} characters long.");
        }
        if (!hasUpper)
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }
        if (!hasLower)
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }
        if (!hasDigit)
        {
            throw new ArgumentException("Password must contain at least one digit.");
        }
        if (!hasSpecial)
        {
            throw new ArgumentException("Password must contain at least one special character.");
        }
    }
    
    private static void ValidateEmail(string? email)
    {
        try
        {
            if (email == null) throw new ArgumentException("Email cannot be empty.");;
            
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
            {
                throw new ArgumentException("Invalid email format.");
            }
        }
        catch
        {
            throw new ArgumentException("Invalid email format.");
        }
    }
}