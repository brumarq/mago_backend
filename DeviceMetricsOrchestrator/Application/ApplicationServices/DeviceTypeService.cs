using System.Net;
using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class DeviceTypeService : IDeviceTypeService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;

    public DeviceTypeService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = _configuration["ApiRequestUris:DeviceTypeBaseUri"];

    }

    public async Task<bool> DeviceTypeExists(int deviceTypeId)
    {
        try
        {
            Console.WriteLine("Device Base URI:");
            Console.WriteLine(_baseUri);
            var response = await _httpClient.GetAsync($"{_baseUri}{deviceTypeId}");

            if (response.IsSuccessStatusCode)
                return true;

            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }

        return false;
    }
}