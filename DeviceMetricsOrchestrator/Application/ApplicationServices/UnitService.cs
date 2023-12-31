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
        private readonly IAuthenticationService _authenticationService;

        public UnitService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAuthenticationService authenticationService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = configuration["ApiRequestUris:UnitBaseUri"]!;
            _authenticationService = authenticationService;
        }

        public async Task<bool> UnitExistsAsync(int unitId)
        {
            try
            {
                if (!_authenticationService.IsLoggedInUser())
                    throw new UnauthorizedException($"The user is not logged in. Please login first.");

                var response = await _httpClient.GetAsync($"{_baseUri}{unitId}");

                return response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound;
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public async Task<UnitResponseDTO> GetUnitByIdAsync(int unitId)
        {
            try
            {
                if (!_authenticationService.IsLoggedInUser())
                    throw new UnauthorizedException($"The user is not logged in. Please login first.");

                if (!await UnitExistsAsync(unitId))
                    throw new NotFoundException($"Unit with id {unitId} does not exist!");

                var response = await _httpClient.GetAsync($"{_baseUri}{unitId}");
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadFromJsonAsync<UnitResponseDTO>();

                return body!;
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }
    }
}
