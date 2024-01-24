using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.ApplicationServices;

public class Auth0ManagementService : IAuth0ManagementService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Auth0ManagementService> _logger;
        
    private ManagementToken _currentToken = new()
    {
        Token = string.Empty,
        ExpirationTime = DateTime.MinValue,
        ExpiresIn = 0
    };

    public Auth0ManagementService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<Auth0ManagementService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ManagementToken> GetToken()
    {
        if (_currentToken.ExpirationTime > DateTime.UtcNow)
        {
            return _currentToken;
        }

        return _currentToken = await FetchNewTokenAsync();
    }

    private async Task<ManagementToken> FetchNewTokenAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Auth0-Management:Domain"]}/oauth/token")
        {
            Content = JsonContent.Create(new TokenRequestDTO
            {
                client_id = _configuration["Auth0-Management:ClientId"],
                client_secret = _configuration["Auth0-Management:ClientSecret"],
                audience = _configuration["Auth0-Management:Audience"],
                grant_type = "client_credentials"
            })
        };

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }

        var token = await response.Content.ReadFromJsonAsync<ManagementToken>();
        if (token == null)
        {
            throw new Exception("Failed to deserialize token response.");
        }

        // Calculate the actual expiration time
        token.ExpirationTime = DateTime.UtcNow.AddSeconds(token.ExpiresIn);

        return token;
    }

    private async Task HandleErrorResponse(HttpResponseMessage response)
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