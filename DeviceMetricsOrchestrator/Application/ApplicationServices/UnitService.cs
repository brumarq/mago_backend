using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Misc;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
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
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{unitId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            using var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound;
        }

        public async Task<UnitResponseDTO> GetUnitByIdAsync(int unitId)
        {
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            if (!await UnitExistsAsync(unitId))
                throw new NotFoundException($"Unit with id {unitId} does not exist!");

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{unitId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<UnitResponseDTO>();

            return body!;
        }
    }
}
