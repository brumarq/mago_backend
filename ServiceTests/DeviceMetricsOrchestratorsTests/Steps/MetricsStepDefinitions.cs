using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TestDomain.Metrics.Field;

namespace DeviceMetricsOrchestratorsTests.Steps;

[Binding]
public sealed class MetricsStepDefinitions
{
    private HttpClient _httpClient;
    private HttpResponseMessage _httpResponseMessage;
    private readonly ScenarioContext _scenarioContext;

    private CreateFieldDTO createdFieldObject = new CreateFieldDTO();

    private static IConfiguration? _configuration;

    [BeforeTestRun]
    public static void InitializeConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public MetricsStepDefinitions(ScenarioContext scenarioContext, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration["DeviceMetricsUrl"]!);
        _scenarioContext = scenarioContext;
    }


    [Given(@"a user is logged in as an admin")]
    public void GivenTheUserIsLoggedInAsAnAdmin()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["AdminJWTToken"]);
    }

    [Given(@"the user is logged in as a client")]
    public void GivenTheUserIsLoggedInAsAClient()
    {
        // Add JWT token for role client to the request headers
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["ClientJWTToken"]);
    }

    [When(@"the user tries to create a field object for unit id (.*) and device type id (.*)")]
    public async Task WhenTheUserTriesToCreateAFieldObjectForUnitId1AndDeviceType1(int unitId, int deviceTypeId)
    {
        var content = new StringContent(
                JsonConvert.SerializeObject(new CreateFieldDTO
                {
                    UnitId = unitId,
                    DeviceTypeId = deviceTypeId,
                    Name = "Awesome field name",
                    Loggable = true
                    
                }),
                Encoding.UTF8,
                "application/json"
            );

        _httpResponseMessage = await _httpClient.PostAsync("/orchestrate/fields/Field", content);

        var response = await _httpResponseMessage.Content.ReadFromJsonAsync<CreateFieldDTO>();
        createdFieldObject = response!;
    }

    [Then(@"the response should return (.*)")]
    public void ThenTheResponseShouldReturn(int statusCode)
    {
        switch (statusCode)
        {
            case 200:
                Assert.AreEqual(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
                break;
            case 201:
                Assert.AreEqual(HttpStatusCode.Created, _httpResponseMessage.StatusCode);
                break;
            case 400:
                Assert.AreEqual(HttpStatusCode.BadRequest, _httpResponseMessage.StatusCode);
                break;
            case 401:
                Assert.AreEqual(HttpStatusCode.Unauthorized, _httpResponseMessage.StatusCode);
                break;
            case 403:
                Assert.AreEqual(HttpStatusCode.Forbidden, _httpResponseMessage.StatusCode);
                break;
        }
    }
}