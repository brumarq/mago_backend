using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TestDomain.Metrics;
using TestDomain.Metrics.Field;

namespace DeviceMetricsOrchestratorsTests.Steps;

[Binding]
public sealed class DeviceMetricsStepDefinitions
{
    private HttpClient _httpClient;
    private HttpResponseMessage _httpResponseMessage;

    private CreateFieldDTO createdFieldObject = new CreateFieldDTO();
    private List<DeviceMetricsResponseDTO> deviceMetricsList = new List<DeviceMetricsResponseDTO>();
    private List<DeviceAggregatedLogsResponseDTO> aggregatedLogsList = new List<DeviceAggregatedLogsResponseDTO>();

    private static IConfiguration _configuration;

    public DeviceMetricsStepDefinitions(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration["DeviceMetricsUrl"]!);
    }

    [Given(@"the user is logged in as (admin|client|forbiddenClient|invalidUser)")]
    public void GivenTheUserIsLoggedInAsX(string role)
    {
        var tokenKeyName = "";
        switch (role)
        {
            case "admin":
                tokenKeyName = "AdminJWTToken";
                break;
            case "client":
                tokenKeyName = "ClientJWTToken";
                break;
            case "forbiddenClient":
                tokenKeyName = "ForbiddenClientJWTToken";
                break;
            case "invalidUser":
                tokenKeyName = "InvalidToken";
                break;
            default:
                throw new InvalidOperationException("Invalid role provided");
        }

        var token = _configuration[tokenKeyName];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
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

        if (_httpResponseMessage.StatusCode != HttpStatusCode.Created)
            return;

        var response = await _httpResponseMessage.Content.ReadFromJsonAsync<CreateFieldDTO>();
        createdFieldObject = response!;
    }

    [When(@"the user tries to retrieve latest device metrics for device id (.*)")]
    public async Task WhenTheUserTriesToRetrieveLatestDeviceMetricsForDevice(int deviceId)
    {
        _httpResponseMessage = await _httpClient.GetAsync($"/orchestrate/device-metrics/DeviceMetrics/{deviceId}");

        if (_httpResponseMessage.StatusCode != HttpStatusCode.OK)
            return;

        var response = await _httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<DeviceMetricsResponseDTO>>();
        deviceMetricsList = response.ToList()!;
    }

    [When(@"the user tries to retrieve aggregated logs for (.*) with device id (.*) and field id (.*) and startDate (.*) and endDate (.*)")]
    public async Task WhenTheUserTriesToRetrieveAggregatedLogsForXWithDeviceIdXAndFieldIdXAndStartDateXAndEndDateX(string aggregatedLogsDateType, int deviceId, int fieldId, string startDate, string endDate)
    {
        if (startDate.Equals("empty"))
            startDate = "";
        if (endDate.Equals("empty"))
            endDate = "";

        if (!string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate))
            _httpResponseMessage = await _httpClient.GetAsync($"/orchestrate/device-metrics/DeviceMetrics/{aggregatedLogsDateType}/{deviceId}/{fieldId}?startDate={startDate}&endDate={endDate}");
        else
            _httpResponseMessage = await _httpClient.GetAsync($"/orchestrate/device-metrics/DeviceMetrics/{aggregatedLogsDateType}/{deviceId}/{fieldId}");

        if (_httpResponseMessage.StatusCode != HttpStatusCode.OK)
            return;

        var response = await _httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<DeviceAggregatedLogsResponseDTO>>();
        aggregatedLogsList = response.ToList()!;
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

    [Then(@"the created field object should be returned")]
    public void AndTheCreatedFieldObjectShouldBeReturned()
    {
        Assert.IsNotNull(createdFieldObject);
    }

    [Then(@"the response should return a list of latest device metrics with (.*) entries")]
    public void AndTheResponseShouldReturnAListOfLatestDeviceMetricsWithXEntries(int nrOfEntries)
    {
        Assert.IsNotNull(deviceMetricsList);
        Assert.AreEqual(deviceMetricsList.Count, nrOfEntries);
    }

    [Then(@"the response should return a list of aggregated logs with (.*) entries")]
    public void AndTheResponseShouldReturnAListOfAggregatedLogsWithXEntries(int nrOfEntries)
    {
        Assert.IsNotNull(aggregatedLogsList);
        Assert.AreEqual(aggregatedLogsList.Count, nrOfEntries);
    }
}