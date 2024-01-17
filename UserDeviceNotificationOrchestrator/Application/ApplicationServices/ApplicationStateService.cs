using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class ApplicationStateService : IApplicationStateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string? _notificationMsBaseUri;
    private readonly string? _deviceMsBaseUri;
    private readonly string? _userMsBaseUri;
    
    public ApplicationStateService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _notificationMsBaseUri = configuration["ApiRequestUris:NotificationBaseUri"];
        _deviceMsBaseUri = configuration["ApiRequestUris:DeviceBaseUri"];
        _userMsBaseUri = configuration["ApiRequestUris:UserBaseUri"];
    }

    public async Task<bool> MicroservicesReady()
    {
        try
        {
            var serviceReadinessUrls = new string[]
            {
                $"{_notificationMsBaseUri}ready",
                $"{_deviceMsBaseUri}ready",
                $"{_userMsBaseUri}ready"
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