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
        private readonly IUsersOnDevicesService _usersOnDevicesService;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _token;

        public AggregatedLogsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IUsersOnDevicesService usersOnDevicesService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:AggregatedLogsBaseUri"]!;
            _deviceService = deviceService;
            _usersOnDevicesService = usersOnDevicesService;
            _httpContextAccessor = httpContextAccessor;
            _token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
        }

        public async Task<IEnumerable<AggregatedLogsResponseDTO>> GetAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
        {
            if (!await _deviceService.DeviceExistsAsync(deviceId))
                throw new NotFoundException($"Device with id {deviceId} does not exist!");

            var loggedInUserId = GetUserId();
            var isAdmin = HasPermission("admin");

            if (!(HasPermission("client") || HasPermission("admin")))
                throw new UnauthorizedException($"The user with id {loggedInUserId} does not have sufficient permissions!");

            var usersOnDevices = await _usersOnDevicesService.GetUsersOnDevicesByUserIdAsync(loggedInUserId!);

            var isAuthorized = usersOnDevices.Any(uod => uod.DeviceId == deviceId) || isAdmin;

            if (!isAuthorized)
                throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{aggregatedLogDateType.ToString()}/{deviceId}/{fieldId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<AggregatedLogsResponseDTO>>();

            if (body == null)
                throw new NotFoundException($"Aggregated logs failed to get retrieved.");

            return body!;
        }

        private string? GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private bool HasPermission(string permission)
        {
            return _httpContextAccessor.HttpContext.User.HasClaim(c => c.Type == "permissions" && c.Value == permission);
        }
    }
}
