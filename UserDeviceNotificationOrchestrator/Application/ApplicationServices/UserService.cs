using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using Application.Exceptions;

namespace Application.ApplicationServices
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _baseUri;
        private readonly string? _baseUriUsersOnDevice;


        public UserService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = configuration["ApiRequestUris:UserBaseUri"];
            _baseUriUsersOnDevice = configuration["ApiRequestUris:UsersOnDeviceUri"];

        }

        public async Task<HttpResponseMessage> GetUserExistenceStatus(string userId)
        {
            string requestUrl = $"{_baseUri}{userId}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
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
        
        public async Task<HttpResponseMessage> DeleteUser(string userId)
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
                var deleteResponse = await _httpClient.DeleteAsync(requestUrl);
                HandleHttpResponse(deleteResponse, userId);
                return deleteResponse;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException($"Exception occurred when deleting user: {e.Message}");
            }
        }
        
        public async Task<HttpResponseMessage> GetUserOnDevice(string userId)
        {
            string requestUrl = $"{_baseUriUsersOnDevice}{userId}";

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
