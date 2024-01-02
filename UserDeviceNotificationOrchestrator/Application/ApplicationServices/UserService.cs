using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.ApplicationServices
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string? _baseUri;
        private readonly string? _baseUriUsersOnDevice;


        public UserService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
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

            // Check if the response is successful
            if (userDevicesResponse.IsSuccessStatusCode)
            {
                var content = await userDevicesResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(content) && content != "[]")
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"User {userId} has devices assigned and cannot be deleted.")
                    };
                }
            }
            else
            {
                return new HttpResponseMessage(userDevicesResponse.StatusCode)
                {
                    Content = new StringContent($"Unable to retrieve devices for user {userId}.")
                };
            }

            string requestUrl = $"{_baseUri}{userId}";

            try
            {
                var deleteResponse = await _httpClient.DeleteAsync(requestUrl);
                return deleteResponse;
            }
            catch (HttpRequestException e)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = $"Exception occurred when deleting user: {e.Message}"
                };
            }
        }

        private async Task<HttpResponseMessage> GetUserOnDevice(string userId)
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


    }
}
