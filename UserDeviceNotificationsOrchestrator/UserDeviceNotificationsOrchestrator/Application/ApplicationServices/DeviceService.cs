using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
