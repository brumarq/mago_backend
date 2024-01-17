using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class ApplicationStateService : IApplicationStateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string? _metricsMsBaseUri;
    private readonly string? _deviceMsBaseUri;
    
    public ApplicationStateService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _metricsMsBaseUri = configuration["ApiRequestUris:MetricsBaseUri"];
        _deviceMsBaseUri = configuration["ApiRequestUris:DeviceBaseUri"];
    }
    
    public async Task<bool> MicroservicesReady()
    {
        try
        {
            var serviceReadinessUrls = new[]
            {
                $"{_metricsMsBaseUri}ready",
                $"{_deviceMsBaseUri}ready"
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
            return true; // All ready
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}