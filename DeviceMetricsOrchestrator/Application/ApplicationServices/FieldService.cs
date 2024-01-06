using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Application.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Application.ApplicationServices
{
    public class FieldService : IFieldService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IUnitService _unitService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;

        public FieldService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IDeviceTypeService deviceTypeService, IUnitService unitService, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = _configuration["ApiRequestUris:FieldBaseUri"]!;
            _deviceTypeService = deviceTypeService;
            _unitService = unitService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }
        public async Task<Dictionary<string,string>> CreateFieldAsync(CreateFieldDTO createFieldDTO)
        {
            if (!_authenticationService.IsLoggedInUser())
                throw new UnauthorizedException($"The user is not logged in. Please login first.");

            if (!_authenticationService.HasPermission("admin"))
                throw new ForbiddenException("The user does not have admin privileges. Only admins are allowed to create fields.");

            await _unitService.CheckUnitExistence(createFieldDTO.UnitId);
            await _deviceTypeService.CheckDeviceTypeExistence(createFieldDTO.DeviceTypeId);

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());

            var json = JsonConvert.SerializeObject(createFieldDTO, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            HttpRequestHelper.CheckStatusAndParseErrorMessageFromJsonData(response);

            var body = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());

            return body!;
        }
    }
}
