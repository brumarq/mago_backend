using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Application.ApplicationServices;

public class DeviceService : IDeviceService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private IUsersOnDevicesService _usersOnDevicesService;
    private readonly IAuthenticationService _authenticationService;

    public DeviceService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IUsersOnDevicesService usersOnDevicesService, IAuthenticationService authenticationService)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = _configuration["ApiRequestUris:DeviceBaseUri"]!;
        _authenticationService = authenticationService;
        _usersOnDevicesService = usersOnDevicesService;
    }

    public async Task<bool> DeviceExistsAsync(int deviceId)
    {
        try
        {
            var loggedInUserId = _authenticationService.GetUserId();

            if (!(_authenticationService.HasPermission("client") || _authenticationService.HasPermission("admin")))
                throw new UnauthorizedException($"The user with id {loggedInUserId} does not have sufficient permissions!");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId)
    {
        try
        {
            var loggedInUserId = _authenticationService.GetUserId();
            var isAdmin = _authenticationService.HasPermission("admin");

            if (!(_authenticationService.HasPermission("client") || _authenticationService.HasPermission("admin")))
                throw new UnauthorizedException($"The user with id {loggedInUserId} does not have sufficient permissions!");

            var usersDevices = await _usersOnDevicesService.GetUsersOnDevicesByUserIdAsync(loggedInUserId!);
            var isAuthorized = usersDevices.Any(uod => uod.DeviceId == deviceId) || isAdmin;

            if (!isAuthorized)
                throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

            if (!await DeviceExistsAsync(deviceId))
                throw new NotFoundException($"Device with id {deviceId} does not exist!");


            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<DeviceResponseDTO>();

            return body!;
        }
        catch(Exception ex)
        {
            throw;
        }
        
    }
}