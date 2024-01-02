using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Application.ApplicationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly string _baseUri;


        public NotificationService(IHttpClientFactory httpClientFactory, IDeviceService deviceService,
            IUserService userService, IConfiguration configuration)
        {
            this._httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            this._deviceService = deviceService;
            this._userService = userService;
            _baseUri = configuration["ApiRequestUris:NotificationBaseUri"];

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

            HttpResponseMessage statusTypeResponseStatus = await GetStatusTypeExistenceStatus(createNotificationDTO.StatusTypeID);
            if (!statusTypeResponseStatus.IsSuccessStatusCode)
            {
                throw new Exception($"Status type check failed: {statusTypeResponseStatus.StatusCode}: {statusTypeResponseStatus.ReasonPhrase}");
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

        public async Task<HttpResponseMessage> GetStatusTypeExistenceStatus(int statusTypeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUri}statusType/{statusTypeId}");
                return response;
            }
            catch (HttpRequestException e)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = $"Exception occurred when status type existence: {e.Message}"
                };
            }
        }
    }
}
