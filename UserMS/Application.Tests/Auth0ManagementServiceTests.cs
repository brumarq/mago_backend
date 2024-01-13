using Moq;
using System.Net;
using System.Net.Http.Json;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Application.Tests;

[TestFixture]
public class Auth0ManagementServiceTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<ILogger<Auth0ManagementService>> _mockLogger = null!;
    private IAuth0ManagementService _auth0ManagementService = null!;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<Auth0ManagementService>>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        _auth0ManagementService = new Auth0ManagementService(_mockHttpClientFactory.Object, _mockConfiguration.Object, _mockLogger.Object);
        
        _mockConfiguration.Setup(c => c["Auth0-Management:Domain"]).Returns("https://example.com");
        _mockConfiguration.Setup(c => c["Auth0-Management:ClientId"]).Returns("clientId");
        _mockConfiguration.Setup(c => c["Auth0-Management:ClientSecret"]).Returns("clientSecret");
        _mockConfiguration.Setup(c => c["Auth0-Management:Audience"]).Returns("audience");
    }
    
    
    [Test]
    public async Task GetToken_TokenValid_ReturnsExistingToken()
    {
        // Assuming ManagementToken is a class with Token and ExpirationTime properties
        var existingToken = new ManagementToken
        {
            Token = "validToken",
            ExpirationTime = DateTime.UtcNow.AddHours(1)
        };

        // Use reflection to set the private _currentToken field
        typeof(Auth0ManagementService)
            .GetField("_currentToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(_auth0ManagementService, existingToken);

        var token = await _auth0ManagementService.GetToken();

        Assert.That(token.Token, Is.EqualTo(existingToken.Token));
    }
    
    [Test]
    public async Task GetToken_TokenInvalid_FetchesNewToken()
    {

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse("https://example.com/oauth/token",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new ManagementTokenResponse { Token = "newToken" })
            });

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        var token = await _auth0ManagementService.GetToken();

        Assert.That(token.Token, Is.EqualTo("newToken"));
    }

    [Test]
    public async Task GetToken_FetchTokenFails_LogsError()
    {
        _mockConfiguration.Setup(c => c["Auth0-Management:Domain"]).Returns("https://example.com");

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse("https://example.com/oauth/token", new HttpResponseMessage(HttpStatusCode.BadRequest));

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        Assert.ThrowsAsync<BadRequestException>(async () => await _auth0ManagementService.GetToken());
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