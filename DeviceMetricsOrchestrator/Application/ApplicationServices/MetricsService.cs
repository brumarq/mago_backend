using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Application.ApplicationServices
{
    public class MetricsService : IMetricsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;
        private readonly string _baseUri;

        public MetricsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _deviceService = deviceService;
            _baseUri = configuration["ApiRequestUris:MetricsBaseUri"];
        }

        public async Task<IEnumerable<MetricsResponseDTO>> GetMetricsForDeviceAsync(int deviceId)
        {
            if (!await _deviceService.DeviceExistsAsync(deviceId))
                throw new NotFoundException($"Device with id {deviceId} does not exist!");

            var response = await _httpClient.GetAsync($"{_baseUri}devices/{deviceId}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<MetricsResponseDTO>>();

            if (body == null)
                throw new NotFoundException("Metrics failed to get retrieved.");

            return body!;
        }
    }
}