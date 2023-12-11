using System.Net;
using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Application.ApplicationServices;

public class DeviceService : IDeviceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly string? _baseUri;

    public DeviceService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
        _baseUri = configuration["ApiRequestUris:DeviceBaseUri"];
    }
    public async Task EnsureDeviceExists(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUri}{deviceId}");

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException("The selected device does not exist.");
                    default:
                        response.EnsureSuccessStatusCode();
                        break;
                    //TODO: add more response status codes
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