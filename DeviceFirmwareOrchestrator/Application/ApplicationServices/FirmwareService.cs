using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Firmware;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class FirmwareService : IFirmwareService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly IDeviceService _deviceService;
    private readonly string _baseUri;

    public FirmwareService(IConfiguration configuration, IHttpClientFactory httpClientFactory,
        IDeviceService deviceService)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _deviceService = deviceService;
        _baseUri = _configuration["ApiRequestUris:FirmwareBaseUri"];
    }

    public async Task<FileSendResponseDTO> CreateFileSendAsync(CreateFileSendDTO newFileSendDto)
    {
        await _deviceService.EnsureDeviceExists(newFileSendDto.DeviceId);

        var response = await _httpClient.PostAsJsonAsync(_baseUri, newFileSendDto);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<FileSendResponseDTO>();
        return body!;
    }

    public async Task<IEnumerable<FileSendResponseDTO>> GetFirmwareHistoryForDeviceAsync(int deviceId)
    {
        await _deviceService.EnsureDeviceExists(deviceId);

        var response = await _httpClient.GetAsync($"{_baseUri}devices/{deviceId}");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<IEnumerable<FileSendResponseDTO>>();
        return body!;
    }
}