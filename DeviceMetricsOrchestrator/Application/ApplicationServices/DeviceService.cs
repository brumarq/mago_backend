using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace Application.ApplicationServices;

public class DeviceService : IDeviceService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;

    public DeviceService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = _configuration["ApiRequestUris:DeviceBaseUri"]!;
    }

    public async Task<bool> DeviceExistsAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUri}{deviceId}");

            return response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error checking device existence: {e.Message}");
            return false;
        }
    }

    public async Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId)
    {
        if (!await DeviceExistsAsync(deviceId))
            throw new NotFoundException($"Device with id {deviceId} does not exist!");

        var response = await _httpClient.GetAsync($"{_baseUri}{deviceId}");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<DeviceResponseDTO>();

        return body!;
    }
}