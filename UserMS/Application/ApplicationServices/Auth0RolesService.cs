using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ApplicationServices;

public class Auth0RolesService: IAuth0RolesService
{
    
    private readonly ILogger<Auth0RolesService> _logger;
    private readonly IAuth0ManagementService _auth0ManagementService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    
    public Auth0RolesService(ILogger<Auth0RolesService> logger, IAuth0ManagementService auth0ManagementService,
        IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _auth0ManagementService = auth0ManagementService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    
     public async Task UnassignRoleAsync(string roleName, string userId)
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
            throw new Auth0Service.UserRoleException($"Error unassigning role from user in Auth0: {errorContent}");
        }
    }

    
    public async Task AssignRole(string roleName, string userId)
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
            throw new Auth0Service.UserRoleException($"Error assigning role to user in Auth0: {errorContent}");
        }
    }
    
    public async Task<string> GetRole(string userId)
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
            throw new Auth0Service.UserRoleException($"Error retrieving roles for user in Auth0: {errorContent}");
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
}