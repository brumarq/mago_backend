using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Misc;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace Application.ApplicationServices
{
    public class UnitService : IUnitService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public UnitService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceService deviceService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = configuration["ApiRequestUris:UnitBaseUri"]!;
        }

        public async Task<bool> UnitExistsAsync(int unitId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUri}{unitId}");

                return response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error checking unit existence: {e.Message}");
                return false;
            }
        }

        public async Task<UnitDTO> GetUnitByIdAsync(int unitId)
        {
            if (!await UnitExistsAsync(unitId))
                throw new NotFoundException($"Unit with id {unitId} does not exist!");

            var response = await _httpClient.GetAsync($"{_baseUri}{unitId}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<UnitDTO>();

            return body!;
        }


    }
}
