using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.ApplicationServices
{
    public class Auth0ManagementService : IAuth0ManagementService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private ManagementToken _currentToken;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Auth0ManagementService> _logger;
        
        public Auth0ManagementService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<Auth0ManagementService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ManagementToken> GetToken()
        {
            if (_currentToken != null && _currentToken.ExpirationTime > DateTime.UtcNow)
                return _currentToken;

            return _currentToken = await FetchNewTokenAsync();
        }

        private async Task<ManagementToken> FetchNewTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Auth0-Management:Domain"]}/oauth/token")
            {
                Content = JsonContent.Create(new ManagementTokenRequestContent
                {
                    client_id = $"{_configuration["Auth0-Management:ClientId"]}",
                    client_secret = $"{_configuration["Auth0-Management:ClientSecret"]}",
                    audience = $"{_configuration["Auth0-Management:Audience"]}",
                    grant_type = "client_credentials"
                })
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error fetching token from Auth0 Management API. Status Code: {StatusCode}", response.StatusCode);
                return new ManagementToken
                {
                    Token = "",
                    ExpirationTime = DateTime.UtcNow.AddSeconds(0)
                };
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<ManagementTokenResponse>();
            return new ManagementToken
            {
                Token = tokenResponse.Token,
                ExpirationTime = DateTime.UtcNow.AddSeconds(86400)
            };
        }
    }

    public class ManagementToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    public class ManagementTokenRequestContent
    {
        public string client_id { get; set; } = string.Empty;
        public string client_secret { get; set; } = string.Empty;
        public string audience { get; set; } = string.Empty;
        public string grant_type { get; set; } = string.Empty;
    }

    public class ManagementTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = string.Empty;
    }
}
