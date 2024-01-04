﻿using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Application.ApplicationServices
{
    public class DeviceService : IDeviceService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpClientFactory _httpClientFactory;      
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly string? _baseUriDevice;
        private readonly string? _baseUriUserOnDevice;

        public DeviceService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IUserService userService, IAuthenticationService authenticationService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _userService = userService;
            _baseUriDevice = configuration["ApiRequestUris:DeviceBaseUri"];
            _baseUriUserOnDevice = configuration["ApiRequestUris:UsersOnDeviceUri"];
            _authenticationService = authenticationService;
        }

        private async Task<HttpResponseMessage> GetDeviceExistenceStatus(int deviceId)
        {
            string requestUrl = $"{_baseUriDevice}{deviceId}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                return await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException e)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = $"Exception occurred when checking device existence: {e.Message}"
                };  
            }
        }
        
        public async void CheckDeviceExistence(int deviceID)
        {
            var deviceResponseStatus = await GetDeviceExistenceStatus(deviceID);
            if (!deviceResponseStatus.IsSuccessStatusCode)
            {
                if (deviceResponseStatus.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException("Device not found");
                else
                    throw new Exception($"Device check failed: {deviceResponseStatus.StatusCode}: {deviceResponseStatus.ReasonPhrase}");
            }
        }

        public async Task<IEnumerable<UserOnDeviceResponseDTO>> GetUserOnDeviceEntryByUserId(string userId)
        {
            _userService.CheckUserExistence(userId);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUriUserOnDevice}{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException("UserOnDevice not found");
                else
                    throw new Exception($"UserOnDevice request failed: {response.StatusCode}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var userOnDeviceResponseDTOs = JsonConvert.DeserializeObject<List<UserOnDeviceResponseDTO>>(content);

            return userOnDeviceResponseDTOs;
        }

        public async Task<UserOnDeviceResponseDTO> CreateUserOnDeviceEntryAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            _userService.CheckUserExistence(createUserOnDeviceDTO.UserId);
            CheckDeviceExistence(createUserOnDeviceDTO.DeviceId);

            var jsonCreateUserOnDeviceDTO = JsonConvert.SerializeObject(createUserOnDeviceDTO);
            var content = new StringContent(jsonCreateUserOnDeviceDTO, Encoding.UTF8, "application/json");


            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _baseUriUserOnDevice) { Content = content };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(request);

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

        public async Task<IActionResult> DeleteUserOnDeviceEntryAsync(string userId, int deviceId)
        {
            try
            {
                _userService.CheckUserExistence(userId);

                CheckDeviceExistence(deviceId);

                UserOnDeviceResponseDTO matchingEntry;

                var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUriUserOnDevice}{userId}");
                getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var devicesFromUserStatus = await _httpClient.SendAsync(getRequest);

                if (devicesFromUserStatus.IsSuccessStatusCode)
                {
                    var responseContent = await devicesFromUserStatus.Content.ReadAsStringAsync();
                    var userOnDeviceResponseDTO = JsonConvert.DeserializeObject<List<UserOnDeviceResponseDTO>>(responseContent);

                    matchingEntry = userOnDeviceResponseDTO.FirstOrDefault(dto => dto.DeviceId == deviceId);
                    if(matchingEntry == null)
                    {
                        return new NotFoundResult();
                    }
                }
                else
                {
                    if (devicesFromUserStatus.StatusCode == HttpStatusCode.NotFound)
                        throw new NotFoundException("UserOnDevice check failed: not found");
                    else
                        throw new Exception($"UserOnDevice check failed: {devicesFromUserStatus.StatusCode}: {devicesFromUserStatus.ReasonPhrase}");
                }

                var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUriUserOnDevice}{matchingEntry.Id}");
                deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var deletionResponse = await _httpClient.SendAsync(deleteRequest);

                if (!deletionResponse.IsSuccessStatusCode)
                {
                    if (deletionResponse.StatusCode == HttpStatusCode.NotFound)
                        return new NotFoundResult();
                    else
                        throw new Exception($"UserOnDevice entry deletion failed: {deletionResponse.StatusCode}: {deletionResponse.ReasonPhrase}");
                }
                else
                {
                    return new NoContentResult();
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }
    }
}
