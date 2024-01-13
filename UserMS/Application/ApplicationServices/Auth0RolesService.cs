using System.Net;
using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["Auth0-Management:Audience"]}users/{userId}/roles")
        {
            Content = JsonContent.Create(new { roles = new[] { roleId } }),
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) await HandleException(response);
    }

    
    public async Task AssignRole(string roleName, string userId)
    {
        var token = await _auth0ManagementService.GetToken();

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Auth0-Management:Audience"]}roles/{_configuration[$"Auth0-Roles:{roleName}"]}/users")
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
        if (!response.IsSuccessStatusCode) await HandleException(response);
    }
    
    public async Task<string> GetRole(string userId)
    {
        var token = await _auth0ManagementService.GetToken();

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["Auth0-Management:Audience"]}users/{userId}/roles")
        {
            Headers =
            {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode) await HandleException(response);

        var roles = await response.Content.ReadFromJsonAsync<List<Role>>();

        // Check if the roles list is not empty, then return the name of the first role
        if (roles != null && roles.Any())
        {
            return roles.First().Name;
        }

        return "";
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