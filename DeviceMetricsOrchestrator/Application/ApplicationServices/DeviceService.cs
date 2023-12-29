using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Application.ApplicationServices;

public class DeviceService : IDeviceService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _token;

    public DeviceService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = _configuration["ApiRequestUris:DeviceBaseUri"]!;
        _httpContextAccessor = httpContextAccessor;
        _token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
    }

    public async Task<bool> DeviceExistsAsync(int deviceId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);

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

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<DeviceResponseDTO>();

        return body!;
    }
}