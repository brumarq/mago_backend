using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Application.Helpers;
using Microsoft.Extensions.Configuration;
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
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;

        public MetricsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _deviceService = deviceService;
            _baseUri = configuration["ApiRequestUris:MetricsBaseUri"]!;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        public async Task<IEnumerable<MetricsResponseDTO>> GetLatestMetricsForDeviceAsync(int deviceId)
        {
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            var loggedInUserId = _authenticationService.GetUserId();

            await _deviceService.CheckDeviceExistence(deviceId);

            if (!await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId!, deviceId))
                throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}metrics/devices/{deviceId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());

            using var response = await _httpClient.SendAsync(request);

            HttpRequestHelper.CheckStatusAndParseErrorMessageFromJsonData(response);

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<MetricsResponseDTO>>();

            if (body == null)
                throw new NotFoundException("Metrics failed to get retrieved.");

            return body!;
        }
    }
}