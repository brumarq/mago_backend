using Application.ApplicationServices.Interfaces;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Application.ApplicationServices
{
    public class AggregatedLogsService : IAggregatedLogsService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDeviceService _deviceService;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public AggregatedLogsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:AggregatedLogsBaseUri"];
            _deviceService = deviceService;
        }

        public async Task<IEnumerable<AggregatedLogsResponseDTO>> GetAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
        {
            if (!await _deviceService.DeviceExistsAsync(deviceId))
                throw new NotFoundException($"Device with id {deviceId} does not exist!");

            var response = await _httpClient.GetAsync($"{_baseUri}{aggregatedLogDateType.ToString()}/{deviceId}/{fieldId}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<AggregatedLogsResponseDTO>>();

            if (body == null)
                throw new NotFoundException($"Aggregated logs failed to get retrieved.");

            return body!;
        }

        public async Task<string> ExportAggregatedLogsCsvAsync(ExportAggregatedLogsCsvDTO exportAggregatedLogsCsvDTO)
        {
            if (!await _deviceService.DeviceExistsAsync(exportAggregatedLogsCsvDTO.DeviceId))
                throw new NotFoundException($"Device with id {exportAggregatedLogsCsvDTO.DeviceId} does not exist!");

            var response = await _httpClient.PostAsJsonAsync($"{_baseUri}export-csv", exportAggregatedLogsCsvDTO);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
