using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.UsersOnDevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Application.ApplicationServices
{
    public class DeviceService : IDeviceService
    {
        private readonly IHttpClientFactory _httpClientFactory;      
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly string? _baseUri;

        public DeviceService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IUserService userService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _userService = userService;
            _baseUri = configuration["ApiRequestUris:DeviceBaseUri"];
        }

        public async Task<HttpResponseMessage> GetDeviceExistenceStatus(int deviceId)
        {
            string requestUrl = $"{_baseUri}{deviceId}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                return response;
            }
            catch (HttpRequestException e)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = $"Exception occurred when checking device existence: {e.Message}"
                };
            }
        }

        public async Task<UserOnDeviceResponseDTO> CreateNotificationAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {

            HttpResponseMessage userResponseStatus = await _userService.GetUserExistenceStatus(createUserOnDeviceDTO.UserId);
            if (!userResponseStatus.IsSuccessStatusCode)
            {
                throw new Exception($"User check failed: {userResponseStatus.StatusCode}: {userResponseStatus.ReasonPhrase}");
            }

            HttpResponseMessage deviceResponseStatus = await GetDeviceExistenceStatus(createUserOnDeviceDTO.DeviceId);
            if (!deviceResponseStatus.IsSuccessStatusCode)
            {
                throw new Exception($"Device check failed: {deviceResponseStatus.StatusCode}: {deviceResponseStatus.ReasonPhrase}");
            }

            var jsonCreateUserOnDeviceDTO = JsonConvert.SerializeObject(createUserOnDeviceDTO);
            var content = new StringContent(jsonCreateUserOnDeviceDTO, Encoding.UTF8, "application/json");


            try
            {
                var response = await _httpClient.PostAsync($"{_baseUri}users-on-devices", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userOnDeviceResponseDTO = JsonConvert.DeserializeObject<UserOnDeviceResponseDTO>(responseContent);
                    return userOnDeviceResponseDTO;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Create UsersOnDevices entry request failed {(int)response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        public async Task<IActionResult> DeleteUserOnDeviceEntryAsync(DeleteUserOnDeviceDTO deleteUserOnDeviceDTO)
        {
            try
            {
                HttpResponseMessage userResponseStatus = await _userService.GetUserExistenceStatus(deleteUserOnDeviceDTO.UserId);
                if (!userResponseStatus.IsSuccessStatusCode)
                {
                    throw new Exception($"User check failed: {userResponseStatus.StatusCode}: {userResponseStatus.ReasonPhrase}");
                }

                HttpResponseMessage deviceResponseStatus = await GetDeviceExistenceStatus(deleteUserOnDeviceDTO.DeviceId);
                if (!deviceResponseStatus.IsSuccessStatusCode)
                {
                    throw new Exception($"Device check failed: {deviceResponseStatus.StatusCode}: {deviceResponseStatus.ReasonPhrase}");
                }

                var response = await _httpClient.DeleteAsync($"{_baseUri}users-on-devices/{deleteUserOnDeviceDTO.Id}");

                if (response.IsSuccessStatusCode)
                {
                    return new NoContentResult();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundResult();
                }
                else
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }
    }
}
