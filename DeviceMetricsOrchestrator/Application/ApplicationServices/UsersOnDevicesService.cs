using Application.ApplicationServices.Interfaces;
using Application.DTOs.Metrics;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Application.ApplicationServices
{
    public class UsersOnDevicesService : IUsersOnDevicesService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDeviceService _deviceService;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _token;

        public UsersOnDevicesService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:UsersOnDevicesBaseUri"]!;
            _deviceService = deviceService;
            _httpContextAccessor = httpContextAccessor;
            _token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
        }

        public async Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId)
        {
            //TODO: add a check here...

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<UsersOnDevicesResponseDTO>>();

            if (body == null)
                throw new NotFoundException($"UsersOnDevices failed to get retrieved.");

            return body!;
        }
    }
}
