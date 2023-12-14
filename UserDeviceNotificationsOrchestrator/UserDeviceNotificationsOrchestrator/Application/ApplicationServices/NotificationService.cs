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

        public async Task<HttpResponseMessage> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
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
                return response;
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Creating notification failed: {e.Message}");
            }
        }
    }
}
