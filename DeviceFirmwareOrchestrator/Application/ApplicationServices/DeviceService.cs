using System.Net;
using System.Net.Http.Headers;
using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class DeviceService : IDeviceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly string? _baseUri;

    public DeviceService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAuthenticationService authenticationService)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = configuration["ApiRequestUris:DeviceBaseUri"];
        _authenticationService = authenticationService;
    }
    
    public async Task EnsureDeviceExists(int deviceId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}/{deviceId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException($"Not Found - Device Microservice: {errorMessage}");
                    case HttpStatusCode.ServiceUnavailable:
                        throw new ServiceUnavailableException($"Service Unavailable - Device Microservice: {errorMessage}");
                    case HttpStatusCode.Unauthorized:
                        throw new UnauthorizedException($"Unauthorized - Device Microservice: {errorMessage}");
                    default:
                        throw new CustomException($"An error occured while checking device existence: {errorMessage}", response.StatusCode);
                }
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}