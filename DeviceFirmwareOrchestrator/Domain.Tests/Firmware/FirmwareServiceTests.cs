using System.Net;
using System.Net.Http.Json;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Firmware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ILogger = NUnit.Framework.Internal.ILogger;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Domain.Tests.Firmware;

[TestFixture]
public class FirmwareServiceTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<IDeviceService> _mockDeviceService;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<FirmwareService>> _mockLogger; 
    private Mock<IAuthenticationService> _authenticationService;

    private HttpClient _httpClient;
    private FirmwareService _firmwareService;
    private MockHttpMessageHandler _fakeHttpMessageHandler;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockDeviceService = new Mock<IDeviceService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _authenticationService = new Mock<IAuthenticationService>();
        _mockConfiguration.Setup(config => config["ApiRequestUris:FirmwareBaseUri"]).Returns("http://localhost:8080/firmware/");

        // Set up HttpClient and HttpClientFactory
        _fakeHttpMessageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

        // Create instance of FirmwareService
        _firmwareService = new FirmwareService(_mockConfiguration.Object, _mockHttpClientFactory.Object, _mockDeviceService.Object, _mockLogger.Object, _authenticationService.Object);
    }

    [Test]
    public async Task CreateFileSendAsync_Success_ReturnsFileSendResponseDTO()
    {
        var newFileSendDto = new CreateFileSendDTO
        {
            DeviceId = 123,
            UserId = 456,
            File = "testfile.txt"
        };

        var expectedResponse = new FileSendResponseDTO
        {
            UpdateStatus = "New",
            DeviceId = newFileSendDto.DeviceId,
            UserId = newFileSendDto.UserId,
            File = newFileSendDto.File,
            CurrParts = 1,
            TotParts = 10
        };
        
        // Mock ensure device exists
        _mockDeviceService.Setup(x => x.EnsureDeviceExists(It.IsAny<int>())).Returns(Task.CompletedTask);
        
        // Mock HTTP response
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedResponse)
        };

        _fakeHttpMessageHandler.SetFakeResponse(httpResponse);

        var result = await _firmwareService.CreateFileSendAsync(newFileSendDto);

        Console.WriteLine(JsonSerializer.Serialize(expectedResponse));
        Console.WriteLine(JsonSerializer.Serialize(result));
        Assert.That(expectedResponse, Is.EqualTo(result));
    }
}