using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Firmware;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.ApplicationServices;

public class FirmwareService : IFirmwareService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FirmwareService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IDeviceService _deviceService;
    private readonly string? _baseUri;
    private readonly IAuthenticationService _authenticationService;

    public FirmwareService(IConfiguration configuration, IHttpClientFactory httpClientFactory,
        IDeviceService deviceService, ILogger<FirmwareService> logger, IAuthenticationService authenticationService)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _deviceService = deviceService;
        _baseUri = configuration["ApiRequestUris:FirmwareBaseUri"];
        _logger = logger;
        _authenticationService = authenticationService;

    }

    public async Task<FileSendResponseDTO> CreateFileSendAsync(CreateFileSendDTO newFileSendDto)
    {
        await _deviceService.EnsureDeviceExists(newFileSendDto.DeviceId);
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}/firmware");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
        
        var json = JsonConvert.SerializeObject(newFileSendDto);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.ServiceUnavailable:
                    throw new ServiceUnavailableException($"Service Unavailable - Firmware Microservice: {errorMessage}");
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException($"Unauthorized - Firmware Microservice: {errorMessage}");
                default:
                    throw new CustomException($"An error occured while creating the new FileSend: {errorMessage}", response.StatusCode);
            }
        }

        var body = await response.Content.ReadFromJsonAsync<FileSendResponseDTO>();
        return body!;
    }

    public async Task<IEnumerable<FileSendResponseDTO>> GetFirmwareHistoryForDeviceAsync(int deviceId)
    {
        _logger.LogError($" The given FirmwareURL: {_baseUri}");
        await _deviceService.EnsureDeviceExists(deviceId);

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}/devices/{deviceId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
        var response = await _httpClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<IEnumerable<FileSendResponseDTO>>();
        return body!;
    }
}