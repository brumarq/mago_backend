using Moq;
using System.Net;
using System.Net.Http.Json;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Application.Tests;

[TestFixture]
public class Auth0ManagementServiceTests
{
    // Mock objects for HttpClientFactory, Configuration, and Logger
    private Mock<IHttpClientFactory> _mockHttpClientFactory = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<ILogger<Auth0ManagementService>> _mockLogger = null!;
    private IAuth0ManagementService _auth0ManagementService = null!;
    private FakeHttpMessageHandler _fakeHttpMessageHandler = null!;
    private HttpClient _fakeHttpClient = null!;

    [SetUp]
    public void Setup()
    {
        // Initializing mock objects
        _mockLogger = new Mock<ILogger<Auth0ManagementService>>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Creating an instance of Auth0ManagementService with mocked dependencies
        _auth0ManagementService = new Auth0ManagementService(_mockHttpClientFactory.Object, _mockConfiguration.Object, _mockLogger.Object);
        
        // Setting up mock configuration with test values
        _mockConfiguration.Setup(c => c["Auth0-Management:Domain"]).Returns("https://example.com");
        _mockConfiguration.Setup(c => c["Auth0-Management:ClientId"]).Returns("clientId");
        _mockConfiguration.Setup(c => c["Auth0-Management:ClientSecret"]).Returns("clientSecret");
        _mockConfiguration.Setup(c => c["Auth0-Management:Audience"]).Returns("audience");
        
        // Setting up the fake HTTP handler and client to mock HTTP requests
        _fakeHttpMessageHandler = new FakeHttpMessageHandler();
        _fakeHttpClient = new HttpClient(_fakeHttpMessageHandler);
    }
    
    [TearDown]
    public void TearDown()
    {
        // Dispose the fake HTTP handler and client after each test
        _fakeHttpMessageHandler.Dispose();
        _fakeHttpClient.Dispose();
    }
    
    
    [Test]
    public async Task GetToken_TokenValid_ReturnsExistingToken()
    {
        // Arrange: Setting up a valid existing token
        var existingToken = new ManagementToken {
            Token = "validToken",
            ExpirationTime = DateTime.UtcNow.AddHours(1)
        };
        
        // Using reflection to set a private field in the service instance
        typeof(Auth0ManagementService)
            .GetField("_currentToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(_auth0ManagementService, existingToken);

        // Act: Attempting to retrieve the token
        var token = await _auth0ManagementService.GetToken();

        // Assert: The token retrieved should be the same as the existing valid token
        Assert.That(token.Token, Is.EqualTo(existingToken.Token));
    }
    
    [Test]
    public async Task GetToken_TokenInvalid_FetchesNewToken()
    {
        _fakeHttpMessageHandler.SetupResponse("https://example.com/oauth/token",
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = JsonContent.Create(new ManagementToken { Token = "newToken" })
            });
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act: Retrieving the token, which should now be a new one
        var token = await _auth0ManagementService.GetToken();

        // Assert: The token retrieved should be the new token provided by the mocked response
        Assert.That(token.Token, Is.EqualTo("newToken"));
    }


    [Test]
    public Task GetToken_FetchTokenFails_LogsError()
    {
        // Arrange: Mocking HTTP response to simulate a failed token fetch
        _fakeHttpMessageHandler.SetupResponse("https://example.com/oauth/token", new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_fakeHttpClient);

        // Act & Assert: Expecting a BadRequestException when the token fetch fails
        Assert.ThrowsAsync<BadRequestException>(async () => await _auth0ManagementService.GetToken());
        return Task.CompletedTask;
    }

    
    private class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses = new Dictionary<string, HttpResponseMessage>();

        public void SetupResponse(string url, HttpResponseMessage responseMessage)
        {
            _responses[url] = responseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_responses.TryGetValue(request.RequestUri.ToString(), out var response))
            {
                return await Task.FromResult(response);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Not Found") };
        }
    }
}