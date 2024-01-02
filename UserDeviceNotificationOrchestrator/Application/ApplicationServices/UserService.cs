using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using Application.Exceptions;

namespace Application.ApplicationServices
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _baseUri;
        private readonly string? _baseUriUsersOnDevice;
        private readonly IAuthenticationService _authenticationService;


        public UserService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IAuthenticationService authenticationService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = configuration["ApiRequestUris:UserBaseUri"];
            _baseUriUsersOnDevice = configuration["ApiRequestUris:UsersOnDeviceUri"];
            _authenticationService = authenticationService;

        }

        public async Task<HttpResponseMessage> GetUserExistenceStatus(string userId)
        {

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(request);
                
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException e)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = $"Exception occurred when checking user existence: {e.Message}"
                };
            }
        }
        
        public async Task DeleteUser(string userId)
        {
            var userDevicesResponse = await GetUserOnDevice(userId);

            HandleHttpResponse(userDevicesResponse, userId);

            var content = await userDevicesResponse.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(content) && content != "[]")
            {
                throw new InvalidOperationException($"User {userId} has devices assigned and cannot be deleted.");
            }

            string requestUrl = $"{_baseUri}{userId}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUri}{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                HandleHttpResponse(response, userId);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                throw;
                
            }
        }
        
        public async Task<HttpResponseMessage> GetUserOnDevice(string userId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUriUsersOnDevice}{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                var response = await _httpClient.SendAsync(request);
                
                response.EnsureSuccessStatusCode();

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
        
        private void HandleHttpResponse(HttpResponseMessage response, string userId)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new ForbiddenException($"Unauthorized access attempt for user {userId}.");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new ForbiddenException($"Forbidden access for user {userId}.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed for user {userId}. Status code: {response.StatusCode}");
            }
        }


    }
}
