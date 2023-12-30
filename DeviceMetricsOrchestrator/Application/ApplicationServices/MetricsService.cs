using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Application.ApplicationServices
{
    public class MetricsService : IMetricsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;
        private readonly string _baseUri;
        private IAuthenticationService _authenticationService;
        private readonly IUsersOnDevicesService _usersOnDevicesService;

        public MetricsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IAuthenticationService authenticationService, IUsersOnDevicesService usersOnDevicesService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _deviceService = deviceService;
            _baseUri = configuration["ApiRequestUris:MetricsBaseUri"]!;
            _authenticationService = authenticationService;
            _usersOnDevicesService = usersOnDevicesService;
        }

        public async Task<IEnumerable<MetricsResponseDTO>> GetMetricsForDeviceAsync(int deviceId)
        {
            try
            {
                if (!await _deviceService.DeviceExistsAsync(deviceId))
                    throw new NotFoundException($"Device with id {deviceId} does not exist!");

                var loggedInUserId = _authenticationService.GetUserId();
                var isClient = _authenticationService.HasPermission("client");
                var isAdmin = _authenticationService.HasPermission("admin");

                if (!(isClient || isAdmin))
                    throw new UnauthorizedException($"The user with id {loggedInUserId} does not have sufficient permissions!");


                var usersDevices = await _usersOnDevicesService.GetUsersOnDevicesByUserIdAsync(loggedInUserId!);
                var isAuthorized = usersDevices.Any(uod => uod.DeviceId == deviceId) || isAdmin;

                if (!isAuthorized)
                    throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}devices/{deviceId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadFromJsonAsync<IEnumerable<MetricsResponseDTO>>();

                if (body == null)
                    throw new NotFoundException("Metrics failed to get retrieved.");

                return body!;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}