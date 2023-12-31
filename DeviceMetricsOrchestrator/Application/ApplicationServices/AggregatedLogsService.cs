using Application.ApplicationServices.Interfaces;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.ApplicationServices
{
    public class AggregatedLogsService : IAggregatedLogsService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDeviceService _deviceService;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUsersOnDevicesService _usersOnDevicesService;

        public AggregatedLogsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IUsersOnDevicesService usersOnDevicesService, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:AggregatedLogsBaseUri"]!;
            _deviceService = deviceService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _usersOnDevicesService = usersOnDevicesService;
        }

        public async Task<IEnumerable<AggregatedLogsResponseDTO>> GetAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
        {
            try
            {
                if (!_authenticationService.IsLoggedInUser())
                    throw new UnauthorizedException($"The user is not logged in. Please login first.");

                if (!await _deviceService.DeviceExistsAsync(deviceId))
                    throw new NotFoundException($"Device with id {deviceId} does not exist!");

                var loggedInUserId = _authenticationService.GetUserId();

                if (!await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId!, deviceId))
                    throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

                // Send request along with a token to the MetricsMS
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{aggregatedLogDateType.ToString()}/{deviceId}/{fieldId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadFromJsonAsync<IEnumerable<AggregatedLogsResponseDTO>>();

                if (body == null)
                    throw new NotFoundException($"Aggregated logs failed to get retrieved.");

                return body!;
            }
            catch(Exception e)
            {
                throw;
            }

        }
    }
}
