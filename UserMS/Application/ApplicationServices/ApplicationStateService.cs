using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class ApplicationStateService : IApplicationStateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string? _auth0Domain;
    
    public ApplicationStateService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _auth0Domain = configuration["Auth0-Management:Domain"];
    }

    public async Task<bool> Auth0Available()
    {
        try
        {
            var serviceReadinessUrls = new string[]
            {
                $"{_auth0Domain}/.well-known/jwks.json",
                $"{_auth0Domain}/.well-known/openid-configuration"
            };

            foreach (var url in serviceReadinessUrls)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return false; // Not Ready
                }
            }
            return true; // Both ready
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}