using System.Net;
using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Enums;
using Application.Exceptions;
using Application.Helpers;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using User = Domain.Entities.User;

namespace Application.ApplicationServices;

public class Auth0Service : IAuth0Service
{
    private readonly ILogger<Auth0Service> _logger;
    private readonly IAuth0ManagementService _auth0ManagementService;
    private readonly IAuth0RolesService _auth0RolesService;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public Auth0Service(ILogger<Auth0Service> logger, IAuth0ManagementService auth0ManagementService,
        IHttpClientFactory httpClientFactory, IConfiguration configuration, IAuth0RolesService auth0RolesService)
    {
        _logger = logger;
        _auth0ManagementService = auth0ManagementService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _auth0RolesService = auth0RolesService;
    }
    
    // 
    // Create User
    //
    public async Task<UserDTO> CreateAuth0UserAsync(CreateUserDTO createUserDto)
    {

        ValidationUtils.ValidateEmail(createUserDto.Email);
        ValidationUtils.ValidatePasswordStrength(createUserDto.Password);

        if (string.IsNullOrWhiteSpace(createUserDto.FamilyName) || string.IsNullOrWhiteSpace(createUserDto.GivenName))
        {
            throw new BadRequestException("Name cannot be empty or whitespace.");
        }
        
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to create a new user in Auth0
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Auth0-Management:Audience"]}users")
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
        if (!response.IsSuccessStatusCode) await HandleException(response);
        
        var auth0User = await response.Content.ReadFromJsonAsync<User>();
        
        var roleName = createUserDto.SysAdmin ? "admin" : "client";
        await _auth0RolesService.AssignRole(roleName, auth0User.UserId);
        
        return new UserDTO
        {
            User = auth0User,
            Role = roleName
        };
    }
    
    // 
    // Update User
    //
    public async Task<UserDTO> UpdateUserAsync(string userId, UpdateUserDTO updateUserDto)
    {
        var updateDetails = new Dictionary<string, object>();
        
        // Validation for each propety in DTO
        if (!string.IsNullOrWhiteSpace(updateUserDto.Email) && !string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            throw new BadRequestException("Email and Password cannot be changed at the same time.");
        }
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
            ValidationUtils.ValidateEmail(updateUserDto.Email); // Validate email
            updateDetails.Add("email", updateUserDto.Email);
        }
        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            ValidationUtils.ValidatePasswordStrength(updateUserDto.Password); // Validate password strength
            updateDetails.Add("password", updateUserDto.Password);
        }
        
        if (updateDetails.Count > 0)
        {
            await UpdateUserDetailsInAuth0(userId, updateDetails);
        }
        
        // No data is returned after updating, making a get request to get the updated user
        return await GetUser(userId);
    }
    
    public async Task<bool> UpdateUserDetailsInAuth0(string userId, object userDetails)
    {
        var token = await _auth0ManagementService.GetToken();
        
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_configuration["Auth0-Management:Audience"]}users/{userId}")
        {
            Content = JsonContent.Create(userDetails),
            Headers = { { "Authorization", $"Bearer {token.Token}" } }
        };

        var response = await client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode) await HandleException(response);
        
        return true;
    }
    
    // Get User/s
    public async Task<UserDTO> GetUser(string userId)
    {
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to get user by ID
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["Auth0-Management:Audience"]}users/{userId}")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode) await HandleException(response);

        var auth0User = await response.Content.ReadFromJsonAsync<User>();
        var role = await _auth0RolesService.GetRole(userId);

        return new UserDTO
        {
            User = auth0User,
            Role = role
        };
    }
    
    public async Task<List<UserCompressedDTO>> GetAllUsers(int pageNumber = 1, int pageSize = 100, UserRole? role = null)
    {
        var allUsersWithRoles = new List<UserCompressedDTO>();

        // Process for each role if UserRole.None is chosen, otherwise process for the selected role
        if (role == null)
        {
            allUsersWithRoles.AddRange(await ProcessRole("admin", pageNumber, pageSize));
            allUsersWithRoles.AddRange(await ProcessRole("client", pageNumber, pageSize));
        }
        else
        {
            allUsersWithRoles.AddRange(await ProcessRole(role.ToString().ToLower(), pageNumber, pageSize));
        }

        return allUsersWithRoles;
    }

    private async Task<List<UserCompressedDTO>> ProcessRole(string roleName, int pageNumber, int pageSize)
    {
        var roleId = _configuration[$"Auth0-Roles:{roleName}"];
        if (string.IsNullOrEmpty(roleId))
        {
            _logger.LogError($"Role ID for {roleName} not found in configuration.");
            return new List<UserCompressedDTO>(); // Return an empty list if role ID is not found
        }

        var users = await GetUsersByRoleId(roleId, pageNumber, pageSize);

        // Transform each user into a UserCompressedDTO with the assigned role.
        return users.Select(user => new UserCompressedDTO
        {
            User = user,
            Role = roleName
        }).ToList();
    }


    public async Task<List<UserCompressed>> GetUsersByRoleId(string roleId, int pageNumber, int pageSize)
    {
        var token = await _auth0ManagementService.GetToken();
        var client = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"{_configuration["Auth0-Management:Audience"]}roles/{roleId}/users?per_page={pageSize}&page={pageNumber - 1}&include_totals=true")
        {
            Headers = { { "Authorization", $"Bearer {token.Token}" } }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await HandleException(response);
        }

        var result = await response.Content.ReadFromJsonAsync<Auth0UsersResponse>();
        return result?.Users ?? new List<UserCompressed>(); // Return the users or an empty list if null
    }
    
    // Delete User
    public async Task<bool> DeleteUserAsync(string userId)
    {
        await GetUser(userId);
        
        // Use _auth0ManagementService to get the access token
        var token = await _auth0ManagementService.GetToken();

        // Prepare the request to delete the user
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["Auth0-Management:Audience"]}users/{userId}")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) await HandleException(response);

        return true;
    }
    
    private async Task HandleException(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponseDto>(errorContent);

        var errorMessage = errorResponse?.Message ?? "An error occurred, but no detailed message was provided.";
        _logger.LogError("Error in Auth0. Status Code: {StatusCode}, Details: {Details}", response.StatusCode, errorMessage);

        throw response.StatusCode switch
        {
            HttpStatusCode.BadRequest => new BadRequestException(errorMessage),
            HttpStatusCode.NotFound => new NotFoundException(errorMessage),
            _ => new CustomException(errorMessage, response.StatusCode)
        };
    }
}