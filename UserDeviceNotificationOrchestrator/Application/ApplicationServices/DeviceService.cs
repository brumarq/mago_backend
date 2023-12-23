using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;


namespace Application.ApplicationServices
{
    public class DeviceService : IDeviceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string? _baseUri;

        public DeviceService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
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


    }
}
