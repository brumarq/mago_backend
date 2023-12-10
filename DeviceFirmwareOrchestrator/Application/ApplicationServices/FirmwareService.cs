using System.Net.Http.Json;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Firmware;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class FirmwareService : IFirmwareService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly IDeviceService _deviceService;
    private readonly string _baseUri;

    public FirmwareService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _deviceService = deviceService;
        _baseUri = _configuration["ApiRequestUris:FirmwareBaseUri"];
    }

    public async Task<FileSendResponseDTO> CreateFileSendAsync(CreateFileSendDTO newFileSendDto)
    {
        var deviceExists = await _deviceService.DeviceExists(newFileSendDto.DeviceId);

        if (!deviceExists)
            throw new Exception("Device does not exist"); //TODO: Fix this abomination
            
        Console.WriteLine("FileSend Base URI:");
        Console.WriteLine(_baseUri);
        
        var response = await _httpClient.PostAsJsonAsync<CreateFileSendDTO>(_baseUri, newFileSendDto);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<FileSendResponseDTO>();

        //TODO: Check if responseBody is not null
        return responseBody;
    }
}