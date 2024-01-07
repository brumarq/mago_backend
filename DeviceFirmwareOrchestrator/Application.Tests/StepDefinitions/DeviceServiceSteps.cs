using System.Net;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using TechTalk.SpecFlow;

namespace Application.Tests.StepDefinitions;

[Binding]
public class DeviceServiceSteps
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;
    private readonly IDeviceService _deviceService;
    private HttpRequestException _caughtException;

    public DeviceServiceSteps()
    {
        // Mock HttpMessageHandler
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            }).Verifiable();

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        // Mock HttpClientFactory
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>())).Returns(httpClient);
        
        // Mock IConfiguration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config["ApiRequestUris:DeviceBaseUri"])
            .Returns("http://device-microservice-service/deviceMS/Device/");
        
        // Mock IAuthenticationService
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        
        // Create instance of DeviceService with mocks
        _deviceService = new DeviceService(_mockConfiguration.Object, _mockHttpClientFactory.Object,
            _mockAuthenticationService.Object);
    }

    [Given(@"the device service is down")]
    public void GivenTheDeviceServiceIsDown()
    {
        // Setup in constructor
    }

    [When(@"I call EnsureDeviceExists")]
    public async void WhenICallEnsureDeviceExists()
    {
        try
        {
            await _deviceService.EnsureDeviceExists(new Random().Next(1, 99));
        }
        catch (HttpRequestException e)
        {
            _caughtException = e;
        }
    }

    [Then(@"an HttpRequestException should be thrown")]
    public void ThenAnHttpRequestExceptionShouldBeThrown()
    {
        Assert.That(_caughtException, Is.Not.Null, "Expected an exception to be thrown, but none was caught");
    }

    [Then(@"response status code should be (.*) \(Service Unavailable\)")]
    public void ThenResponseStatusCodeShouldBeServiceUnavailable(int statusCode)
    {
        Assert.Multiple(() =>
        {
            Assert.That(_caughtException?.StatusCode, Is.Not.Null);
            // Assert that response status code is 503
            Assert.That((int)_caughtException?.StatusCode!, Is.EqualTo(statusCode), $"Expected a status code of {statusCode}, but got {(int)_caughtException.StatusCode}.");
        });
    }
}