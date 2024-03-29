﻿using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Application.Helpers;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

        public AggregatedLogsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:AggregatedLogsBaseUri"]!;
            _deviceService = deviceService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        public async Task<IEnumerable<AggregatedLogsResponseDTO>> GetAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string startDate, string endDate, int pageNumber, int pageSize)
        {
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            await _deviceService.CheckDeviceExistence(deviceId);

            var loggedInUserId = _authenticationService.GetUserId();

            if (!await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId!, deviceId))
                throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access device with id {deviceId}");

            string apiUrl;
            if (!string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate))
                apiUrl = $"{_baseUri}{aggregatedLogDateType.ToString()}/{deviceId}/{fieldId}?start_date={startDate}&end_date={endDate}&page_number={pageNumber}&page_size={pageSize}";
            else
                apiUrl = $"{_baseUri}{aggregatedLogDateType.ToString()}/{deviceId}/{fieldId}?page_number={pageNumber}&page_size={pageSize}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());

            var response = await _httpClient.SendAsync(request);

            HttpRequestHelper.CheckStatusAndParseErrorMessageFromJsonData(response);

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<AggregatedLogsResponseDTO>>();

            if (body == null)
                throw new NotFoundException($"Aggregated logs failed to get retrieved.");

            return body!;
        }
    }
}
