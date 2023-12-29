using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Application.ApplicationServices
{
    public class MetricsService : IMetricsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;
        private readonly string _baseUri;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _token;

        public MetricsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _deviceService = deviceService;
            _baseUri = configuration["ApiRequestUris:MetricsBaseUri"]!;
            _httpContextAccessor = httpContextAccessor;
            _token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
        }

        public async Task<IEnumerable<MetricsResponseDTO>> GetMetricsForDeviceAsync(int deviceId)
        {
            if (!await _deviceService.DeviceExistsAsync(deviceId))
                throw new NotFoundException($"Device with id {deviceId} does not exist!");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}devices/{deviceId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<MetricsResponseDTO>>();

            if (body == null)
                throw new NotFoundException("Metrics failed to get retrieved.");

            return body!;
        }
    }
}