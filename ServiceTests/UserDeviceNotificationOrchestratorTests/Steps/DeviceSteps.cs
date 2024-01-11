using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Microsoft.Extensions.Configuration;

namespace ServiceTests.Steps;

[Binding]
public sealed class DeviceSteps
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

    public DeviceSteps(ScenarioContext scenarioContext, HttpClient httpClient)
    {
        _scenarioContext = scenarioContext;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["AdminJWTToken"]);
    }

    [Given(@"the request is set to User Device Notification Orchestrator")]
    public void GivenTheRequestIsSetToLocalhostOnPort9090()
    {
        _baseUrl = _configuration["UserDeviceNotificationURL"];
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    [When("a get request is made to get notification by device")]
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
    
    [When("a get request is made to get notification by device when device does not exist")]
    public async Task WhenTheTwoNumbersAreAdded1()
    {
        try
        {
            _lastResponse = await _httpClient.GetAsync($"/orchestrator/notification/device/2");
            _lastResponse.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            _scenarioContext["error"] = e.Message;
        }
    }
    
    
    [When("a get request is made to get notification")]
    public async Task WhenTheTwoNumbersAreAdded2()
    {
        try
        {
            _lastResponse = await _httpClient.GetAsync($"/orchestrator/notification/1");
            _lastResponse.EnsureSuccessStatusCode();
            string responseBody = await _lastResponse.Content.ReadAsStringAsync();
            _jsonObjectResult = JObject.Parse(responseBody);
        }
        catch (HttpRequestException e)
        {
            _scenarioContext["error"] = e.Message;
        }
    }
    
    [When("a get request is made to get notification that does not exist")]
    public async Task WhenTheTwoNumbersAreAdded3()
    {
        try
        {
            _lastResponse = await _httpClient.GetAsync($"/orchestrator/notification/99999");
            _lastResponse.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            _scenarioContext["error"] = e.Message;
        }
    }
    
    [When("a post request is made to create a notification with existing device and notification type")]
    public async Task WhenTheTwoNumbersAreAdded4()
    {
        try
        {
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    deviceID = 1,
                    statusTypeID = 1,
                    message = "Machine is down"
                }), Encoding.UTF8, "application/json");

            _lastResponse = await _httpClient.PostAsync("/orchestrator/notification", jsonContent);
            _lastResponse.EnsureSuccessStatusCode();

            var responseContent = await _lastResponse.Content.ReadAsStringAsync();
            _jsonObjectResult = JObject.Parse(responseContent);
        }
        catch (HttpRequestException e)
        {
            _scenarioContext["error"] = e.Message;
        }
    }
    
    [When("a post request is made to create a notification with non-existent device")]
    public async Task WhenTheTwoNumbersAreAdded5()
    {
        try
        {
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    deviceID = 99999,
                    statusTypeID = 1,
                    message = "Machine is down"
                }), Encoding.UTF8, "application/json");

            _lastResponse = await _httpClient.PostAsync("/orchestrator/notification", jsonContent);
            _lastResponse.EnsureSuccessStatusCode();

            var responseContent = await _lastResponse.Content.ReadAsStringAsync();
            _jsonObjectResult = JObject.Parse(responseContent);
        }
        catch (HttpRequestException e)
        {
            _scenarioContext["error"] = e.Message;
        }
    }
    
    [When("post request is made to create a notification with non-existent notification type")]
    public async Task WhenTheTwoNumbersAreAdded6()
    {
        try
        {
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    deviceID = 1,
                    statusTypeID = 99999,
                    message = "Machine is down"
                }), Encoding.UTF8, "application/json");

            _lastResponse = await _httpClient.PostAsync("/orchestrator/notification", jsonContent);
            _lastResponse.EnsureSuccessStatusCode();

            var responseContent = await _lastResponse.Content.ReadAsStringAsync();
            _jsonObjectResult = JObject.Parse(responseContent);
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
    
    [Then("an object with id 1 should exist")]
    public void ThenAnObjectWithId1ShouldExist()
    {
        if (_scenarioContext.ContainsKey("error"))
        {
            Assert.Fail($"An error occurred during HTTP request: {_scenarioContext["error"]}");
        }
        else
        {
            bool idExists = _jsonObjectResult["id"]?.Value<int>() == 1;
            Assert.IsTrue(idExists, "No object with id 1 was found in the response.");
        }
    }
    
    [Then(@"the response object should contain ""(.*)"" with value ""(.*)""")]
    public void ThenTheResponseObjectShouldContainWithValue(string property, string value)
    {
        var actualValue = _jsonObjectResult[property];

        Assert.IsNotNull(actualValue, $"The key '{property}' was not found in the response.");
        Assert.AreEqual(value, actualValue.ToString(), $"The value for '{property}' did not match.");
    }



    
    [Then("the response code should be (.*)")]
    public void ThenTheResponseCodeShouldBe(int expectedStatusCode)
    {
            Assert.AreEqual(expectedStatusCode, (int)_lastResponse.StatusCode, "The response status code is not as expected.");
    }

}
