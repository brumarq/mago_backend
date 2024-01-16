using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class ApplicationStateService : IApplicationStateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string? _firmwareMsBaseUri;
    private readonly string? _deviceMsBaseUri;
    
    public ApplicationStateService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _firmwareMsBaseUri = configuration["ApiRequestUris:FirmwareBaseUri"];
        _deviceMsBaseUri = configuration["ApiRequestUris:DeviceBaseUri"];
    }

    public async Task<bool> MicroservicesReady()
    {
        try
        {
            var serviceReadinessUrls = new string[]
            {
                $"{_firmwareMsBaseUri}/ready",
                $"{_deviceMsBaseUri}/ready"
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