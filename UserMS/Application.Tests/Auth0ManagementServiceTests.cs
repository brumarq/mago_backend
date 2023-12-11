using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;


namespace Application.Tests;

[TestFixture]
public class Auth0ManagementServiceTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<Auth0ManagementService>> _mockLogger;
    private IAuth0ManagementService _auth0ManagementService;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<Auth0ManagementService>>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        _auth0ManagementService = new Auth0ManagementService(_mockHttpClientFactory.Object, _mockConfiguration.Object, _mockLogger.Object);    
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
            .SetValue(_auth0ManagementService, existingToken);

        var token = await _auth0ManagementService.GetToken();

        Assert.AreEqual(existingToken.Token, token.Token);
    }
    
    [Test]
    public async Task GetToken_TokenInvalid_FetchesNewToken()
    {
        _mockConfiguration.Setup(c => c["Auth0-Management:Domain"]).Returns("https://example.com");
        _mockConfiguration.Setup(c => c["Auth0-Management:ClientId"]).Returns("clientId");
        _mockConfiguration.Setup(c => c["Auth0-Management:ClientSecret"]).Returns("clientSecret");
        _mockConfiguration.Setup(c => c["Auth0-Management:Audience"]).Returns("audience");

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse("https://example.com/oauth/token",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new ManagementTokenResponse { Token = "newToken" })
            });

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        var token = await _auth0ManagementService.GetToken();

        Assert.AreEqual("newToken", token.Token);
    }

    [Test]
    public async Task GetToken_FetchTokenFails_LogsError()
    {
        _mockConfiguration.Setup(c => c["Auth0-Management:Domain"]).Returns("https://example.com");

        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        fakeHttpMessageHandler.SetupResponse("https://example.com/oauth/token", new HttpResponseMessage(HttpStatusCode.BadRequest));

        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

        var token = await _auth0ManagementService.GetToken();

        Assert.IsEmpty(token.Token);
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