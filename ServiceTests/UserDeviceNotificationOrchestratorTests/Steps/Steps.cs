using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

namespace ServiceTests.Steps
{
    [Binding]
    public sealed class Steps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HttpClient _httpClient;
        private JObject _jsonObjectResult;
        private JArray _jsonArrayResult;
        private HttpResponseMessage _lastResponse;
        private static IConfiguration _configuration;

        public Steps(ScenarioContext scenarioContext, HttpClient httpClient, IConfiguration configuration)
        {
            _scenarioContext = scenarioContext;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [Given(@"the request is set to User Device Notification Orchestrator")]
        public void GivenTheRequestIsSetToUserDeviceNotificationOrchestrator()
        {
            var baseUrl = _configuration["UserDeviceNotificationURL"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Base URL is not set in the appsettings.json file.");
            }

            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new UriFormatException("The base URL from the configuration is not a well-formed URI.");
            }

            _httpClient.BaseAddress = new Uri(baseUrl);
        }
        
        [Given(@"I am logged in as (admin|client1|client2|client3)")]
        public void GivenIAmLoggedInAs(string role)
        {
            var tokenKey = role switch
            {
                "admin" => "AdminJWTToken",
                "client1" => "Client1JWTToken",
                "client2" => "Client2JWTToken",
                "client3" => "Client3JWTToken",
                _ => throw new InvalidOperationException($"Unrecognized role: {role}")
            };

            var token = _configuration[tokenKey];
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException($"JWT token for role '{role}' is not set in the appsettings.json file.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _scenarioContext["AuthToken"] = token;
        }

        [When(@"a get request is made to (.*)")]
        public async Task WhenGetRequestIsMade(string endpoint)
        {
            try
            {
                _lastResponse = await _httpClient.GetAsync(endpoint);
                ProcessResponse();
            }
            catch (HttpRequestException e)
            {
                _scenarioContext["error"] = e.Message;
            }
        }

        [When(@"a post request is made to (.*) with payload:")]
        public async Task WhenPostRequestIsMade(string endpoint, string payload)
        {
            try
            {
                var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
                _lastResponse = await _httpClient.PostAsync(endpoint, jsonContent);
                ProcessResponse();
            }
            catch (HttpRequestException e)
            {
                _scenarioContext["error"] = e.Message;
            }
        }
        
        [When(@"a delete request is made to (.*)")]
        public async Task WhenDeleteRequestIsMade(string endpoint)
        {
            try
            {
                _lastResponse = await _httpClient.DeleteAsync(endpoint);
            }
            catch (HttpRequestException e)
            {
                _scenarioContext["error"] = e.Message;
            }
        }

        [Then("an item with id (.*) should exist")]
        public void ThenAnItemWithIdShouldExist(int id)
        {
            Assert.IsNotNull(_jsonArrayResult, "The response does not contain a JSON array.");
            bool idExists = _jsonArrayResult.Any(item => item["id"]?.Value<int>() == id);
            Assert.IsTrue(idExists, $"No item with id {id} was found in the response.");
        }

        [Then("an object with id (.*) should exist")]
        public void ThenAnObjectWithIdShouldExist(int id)
        {
            Assert.IsNotNull(_jsonObjectResult, "The response does not contain a JSON object.");
            bool idExists = _jsonObjectResult["id"]?.Value<int>() == id;
            Assert.IsTrue(idExists, $"No object with id {id} was found in the response.");
        }

        [Then(@"the response object should contain ""(.*)"" with value ""(.*)""")]
        public void ThenTheResponseObjectShouldContainWithValue(string property, string value)
        {
            Assert.IsNotNull(_jsonObjectResult, "The response does not contain a JSON object.");
            var actualValue = _jsonObjectResult[property];
            Assert.IsNotNull(actualValue, $"The key '{property}' was not found in the response.");
            Assert.AreEqual(value, actualValue.ToString(), $"The value for '{property}' did not match.");
        }

        [Then("the response code should be (.*)")]
        public void ThenTheResponseCodeShouldBe(int expectedStatusCode)
        {
            Assert.AreEqual(expectedStatusCode, (int)_lastResponse.StatusCode, "The response status code is not as expected.");
        }
        
        
        // Helpers
        private void ProcessResponse()
        {
            _lastResponse.EnsureSuccessStatusCode();
            var responseBody = _lastResponse.Content.ReadAsStringAsync().Result;

            if (responseBody.StartsWith("["))
                _jsonArrayResult = JArray.Parse(responseBody);
            else
                _jsonObjectResult = JObject.Parse(responseBody);
        }
    }
}
