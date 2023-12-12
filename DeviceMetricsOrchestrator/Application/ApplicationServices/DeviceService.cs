using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
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
        _baseUri = _configuration["ApiRequestUris:DeviceBaseUri"];
    }

    public async Task<bool> DeviceExists(int deviceId)
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
}