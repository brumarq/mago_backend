using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net;
using Application.Helpers;
using System.Net.Http.Json;

namespace Application.ApplicationServices
{
    public class DeviceTypeService : IDeviceTypeService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;

        public DeviceTypeService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:DeviceTypeBaseUri"]!;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        public async Task CheckDeviceTypeExistence(int deviceTypeId)
        {
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceTypeId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException($"This user does not have access to view device type {deviceTypeId}");

            if (!response.IsSuccessStatusCode || response == null)
                throw new NotFoundException($"Device with id {deviceTypeId} does not exist.");
        }

        public async Task<DeviceTypeResponseDTO> GetDeviceTypeByIdAsync(int deviceTypeId)
        {
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            await CheckDeviceTypeExistence(deviceTypeId);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{deviceTypeId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);

            HttpRequestHelper.CheckStatusAndParseErrorMessageFromJsonData(response);

            var body = await response.Content.ReadFromJsonAsync<DeviceTypeResponseDTO>();

            if (body == null)
                throw new NotFoundException("Failed to retrieve device by id");

            return body!;
        }
    }
}
