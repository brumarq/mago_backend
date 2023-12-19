using Application.ApplicationServices.Interfaces;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using System.Web.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Net;
using Newtonsoft.Json.Serialization;

namespace Application.ApplicationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly string _baseUri;


        public NotificationService(IHttpClientFactory httpClientFactory, IDeviceService deviceService, IUserService userService, IConfiguration configuration)
        {
            this._httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            this._deviceService = deviceService;
            this._userService = userService;
            _baseUri = configuration["ApiRequestUris:NotificationBaseUri"];

        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsForUserOnStatusTypeByUserIdAsync(int userId)
        {
            HttpResponseMessage userResponseStatus = await _userService.GetUserExistenceStatus(userId);
            if (!userResponseStatus.IsSuccessStatusCode)
            {
                throw new Exception($"Device check failed: {userResponseStatus.StatusCode}: {userResponseStatus.ReasonPhrase}");
            }

            try
            {
                //"/user-on-statustype/user/{userId}/device-ids"
                var deviceIdsResponse = await _httpClient.GetAsync($"{_baseUri}user-on-statustype/user/{userId}/device-ids");
                string content = await deviceIdsResponse.Content.ReadAsStringAsync();
                List<int> deviceIds = JsonConvert.DeserializeObject<List<int>>(content);
                foreach (var deviceId in deviceIds)
                {
                    var deviceResponseStatus = await _deviceService.GetDeviceExistenceStatus(deviceId);
                    if (!deviceResponseStatus.IsSuccessStatusCode)
                    {
                        throw new Exception($"Device check failed: {deviceResponseStatus.StatusCode}: {deviceResponseStatus.ReasonPhrase}");
                    }
                }

                var notificatinonsResponse = await _httpClient.GetAsync($"{_baseUri}users/{userId}");
                if (notificatinonsResponse.IsSuccessStatusCode)
                {
                    var responseContent = await notificatinonsResponse.Content.ReadAsStringAsync();
                    var notificationResponseDTOs = JsonConvert.DeserializeObject<List<NotificationResponseDTO>>(responseContent);
                    return notificationResponseDTOs;
                }
                else
                {
                    var errorContent = await notificatinonsResponse.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Get notifications request failed {(int)notificatinonsResponse.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceIdAsync(int deviceId)
        {
            HttpResponseMessage deviceResponseStatus = await _deviceService.GetDeviceExistenceStatus(deviceId);
            if (!deviceResponseStatus.IsSuccessStatusCode)
            {
                throw new Exception($"Device check failed: {deviceResponseStatus.StatusCode}: {deviceResponseStatus.ReasonPhrase}");
            }
                
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUri}device/{deviceId}");
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

        public async Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            HttpResponseMessage deviceResponseStatus = await _deviceService.GetDeviceExistenceStatus(createNotificationDTO.DeviceID);
            if (!deviceResponseStatus.IsSuccessStatusCode)
            {
                throw new Exception($"Device check failed: {deviceResponseStatus.StatusCode}: {deviceResponseStatus.ReasonPhrase}");
            }

            var jsonNotificationDTO = JsonConvert.SerializeObject(createNotificationDTO);
            var content = new StringContent(jsonNotificationDTO, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_baseUri, content);
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


    }
}
