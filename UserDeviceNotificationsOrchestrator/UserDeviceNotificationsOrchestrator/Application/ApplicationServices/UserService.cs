using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices
{
    public class UserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string? _baseUri;

        public UserService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = configuration["ApiRequestUris:UserBaseUri"];
        }

        public async Task<bool> CheckDeviceExists(int userId)
        {
            string requestUrl = $"{_baseUri}{userId}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error checking user existence: {e.Message}");
                throw;
            }
        }
    }
}
