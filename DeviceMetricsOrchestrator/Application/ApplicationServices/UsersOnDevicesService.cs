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
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IAuthenticationService _authenticationService;

        public UsersOnDevicesService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAuthenticationService authenticationService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:UsersOnDevicesBaseUri"]!;
            _authenticationService = authenticationService;
        }

        public async Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId)
        {           
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            var loggedInUserId = _authenticationService.GetUserId();
            var isAdmin = _authenticationService.HasPermission("admin");

            if (!loggedInUserId.Equals(userId) && !isAdmin)
                throw new ForbiddenException($"The user with id {loggedInUserId} does not have permission to access user {userId}'s information");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IEnumerable<UsersOnDevicesResponseDTO>>();

            if (body == null)
                throw new NotFoundException($"UsersOnDevices failed to get retrieved.");

            return body!;
        }
    }
}
