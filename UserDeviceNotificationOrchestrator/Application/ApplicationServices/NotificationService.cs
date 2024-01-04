﻿using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Application.ApplicationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly string _baseUri;


        public NotificationService(IHttpClientFactory httpClientFactory, IDeviceService deviceService,
            IUserService userService, IConfiguration configuration, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
        {
            this._httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            this._deviceService = deviceService;
            this._userService = userService;
            _baseUri = configuration["ApiRequestUris:NotificationBaseUri"];
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceIdAsync(int deviceId)
        {
            _deviceService.CheckDeviceExistence(deviceId);

            try
            {
                var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}device/{deviceId}");
                getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(getRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var notificationResponseDTOs = JsonConvert.DeserializeObject<List<NotificationResponseDTO>>(responseContent);
                    return notificationResponseDTOs;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Get notifications request failed {(int)response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        public async Task<NotificationResponseDTO> GetNotificationByIdAsync(int id)
        {
            try
            {
                var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{id}");
                getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(getRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var notificationResponseDTO = JsonConvert.DeserializeObject<NotificationResponseDTO>(responseContent);

                    string loggedInUser = _authenticationService.GetUserId();
                    bool isAccessibleToUser = await _authorizationService.IsNotificationAccessibleToUser(loggedInUser, notificationResponseDTO);
                    if (!isAccessibleToUser)
                        throw new ForbiddenException("User not allowed to view this notification");
                    
                    return notificationResponseDTO;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Get notification by id request failed {(int)response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        public async Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            _deviceService.CheckDeviceExistence(createNotificationDTO.DeviceID);

            CheckStatusTypeExistence(createNotificationDTO.StatusTypeID);

            var jsonNotificationDTO = JsonConvert.SerializeObject(createNotificationDTO);
            var content = new StringContent(jsonNotificationDTO, Encoding.UTF8, "application/json");

            try
            {
                var postRequest = new HttpRequestMessage(HttpMethod.Post, _baseUri) { Content = content };
                postRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(postRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var notificationResponseDTO = JsonConvert.DeserializeObject<NotificationResponseDTO>(responseContent);
                    return notificationResponseDTO;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Create notification request failed {(int)response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        private async Task<HttpResponseMessage> GetStatusTypeExistenceStatus(int statusTypeId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}statusType/{statusTypeId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                return await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException e)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = $"Exception occurred when status type existence: {e.Message}"
                };
            }
        }

        public async void CheckStatusTypeExistence(int statusTypeId)
        {
            var statusTypeResponseStatus = await GetStatusTypeExistenceStatus(statusTypeId);
            if (!statusTypeResponseStatus.IsSuccessStatusCode)
            {
                if (statusTypeResponseStatus.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException("Status type not found");
                else
                    throw new Exception($"Status type check failed: {statusTypeResponseStatus.StatusCode}: {statusTypeResponseStatus.ReasonPhrase}");
            }
        }
    }
}
