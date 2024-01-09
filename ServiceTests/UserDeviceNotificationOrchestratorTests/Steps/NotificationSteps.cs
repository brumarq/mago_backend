using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Microsoft.Extensions.Configuration;

namespace ServiceTests.Steps;

[Binding]
public sealed class NotificationSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly HttpClient _httpClient;
    private string _baseUrl;
    private int _firstNumber;
    private int _secondNumber;
    private int _result;
    private JObject _jsonObjectResult;
    private JArray _jsonArrayResult;
    private HttpResponseMessage _lastResponse;

    private static IConfiguration _configuration;

    [BeforeTestRun]
    public static void InitializeConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public NotificationSteps(ScenarioContext scenarioContext, HttpClient httpClient)
    {
        _scenarioContext = scenarioContext;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["AdminJWTToken"]);
    }
    
    [Given(@"the request is set to localhost on port 8080")]
    public void GivenTheRequestIsSetToLocalhostOnPort8080()
    {
        _baseUrl = "http://localhost:8080";
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    [Given(@"the request is set to User Device Notification Orchestrator")]
    public void GivenTheRequestIsSetToLocalhostOnPort9090()
    {
        _baseUrl = _configuration["UserDeviceNotificationURL"];
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    [When("a get request is made to get a notification")]
    public async Task WhenTheTwoNumbersAreAdded()
    {
        try
        {
            _lastResponse = await _httpClient.GetAsync($"/orchestrator/notification/device/1");
            _lastResponse.EnsureSuccessStatusCode();
            string responseBody = await _lastResponse.Content.ReadAsStringAsync();
            _jsonArrayResult = JArray.Parse(responseBody);
        }
        catch (HttpRequestException e)
        {
            _scenarioContext["error"] = e.Message;
        }
    }


    [Then("an item with id 1 should exist")]
    public void ThenAnItemWithId1ShouldExist()
    {
        if (_scenarioContext.ContainsKey("error"))
        {
            Assert.Fail($"An error occurred during HTTP request: {_scenarioContext["error"]}");
        }
        else
        {
            bool idExists = _jsonArrayResult.Any(item => item["id"]?.Value<int>() == 1);
            Assert.IsTrue(idExists, "No item with id 1 was found in the response.");
        }
    }

    
    [Then("the response code should be (.*)")]
    public void ThenTheResponseCodeShouldBe(int expectedStatusCode)
    {
        if (_scenarioContext.ContainsKey("error"))
        {
            Assert.Fail($"An error occurred during HTTP request: {_scenarioContext["error"]}");
        }
        else
        {
            Assert.AreEqual(expectedStatusCode, (int)_lastResponse.StatusCode, "The response status code is not as expected.");
        }
    }

}
