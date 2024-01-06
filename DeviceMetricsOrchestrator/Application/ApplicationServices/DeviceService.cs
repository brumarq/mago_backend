using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.Exceptions;
using Application.Helpers;
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
    private readonly IAuthenticationService _authenticationService;
    private readonly IAuthorizationService _authorizationService;

    public DeviceService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = _configuration["ApiRequestUris:DeviceBaseUri"]!;
        _authenticationService = authenticationService;
        _authorizationService = authorizationService;
    }

    public async Task CheckDeviceExistence(int deviceId)
    {
        if (!_authenticationService.IsLoggedInUser())
            throw new UnauthorizedException($"The user is not logged in. Please login first.");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedException($"This user does not have access to view device {deviceId}");

        if (!response.IsSuccessStatusCode || response == null)
            throw new NotFoundException($"Device with id {deviceId} does not exist.");
    }

    public async Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId)
    {
        if (!_authenticationService.IsLoggedInUser())
            throw new UnauthorizedException($"The user is not logged in. Please login first.");

        await CheckDeviceExistence(deviceId);

        var loggedInUserId = _authenticationService.GetUserId();

        if (!await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId!, deviceId))
            throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
        var response = await _httpClient.SendAsync(request);

        HttpRequestHelper.CheckStatusAndParseErrorMessageFromJsonData(response);

        var body = await response.Content.ReadFromJsonAsync<DeviceResponseDTO>();

        if (body == null)
            throw new NotFoundException("Failed to retrieve device by id");

        return body!;
    }
}