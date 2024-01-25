using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using TestDomain.Firmware;

namespace DeviceFirmwareOrchestratorTests.Steps;

[Binding]
public sealed class FirmwareStepDefinitions
{
    private HttpClient _client;
    private HttpResponseMessage _response;
    private List<FileSendResponseDTO> createdFileSends = new();
    private List<FileSendResponseDTO> retrievedFileSends = new();

    private static IConfiguration _configuration;

    [BeforeTestRun]
    public static void InitializeConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public FirmwareStepDefinitions(HttpClient httpClient)
    {
        _client = httpClient;
        _client.BaseAddress = new Uri(_configuration["FirmwareDeviceUrl"]);
    }

    [Given(@"the user is logged in as an admin")]
    public void GivenTheUserIsLoggedInAsAnAdmin()
    {
        // Add JWT token for role admin to the request headers
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["AdminJWTToken"]);
    }

    [Given(@"the user is logged in as a client")]
    public void GivenTheUserIsLoggedInAsAClient()
    {
        // Add JWT token for role client to the request headers
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["ClientJWTToken"]);
    }

    [When(@"the user attempts to create multiple FileSends for an existing device with id (.*)")]
    public async Task WhenTheUserAttemptsToCreateMultipleFileSendsForAnExistingDeviceWithId(int deviceId)
    {
        // Attempt to create 3 Firmware FileSends
        for (int i = 0; i < 12; i++)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(new CreateFileSendDTO
                {
                    DeviceId = deviceId,
                    File = "TestFile"
                }),
                Encoding.UTF8,
                "application/json"
            );

            _response = await _client.PostAsync(
                "/orchestrate/device-firmware/Firmware",
                content
            );

            if (_response.StatusCode != HttpStatusCode.Created) continue;
            
            var createdFileSend = await _response.Content.ReadFromJsonAsync<FileSendResponseDTO>();
            if (createdFileSend != null) createdFileSends.Add(createdFileSend);
        }
    }

    [Then(@"the response should contain status code (.*)")]
    public void ThenTheResponseShouldContainStatusCode(int statusCode)
    {
        switch (statusCode)
        {
            case 200:
                Assert.AreEqual(HttpStatusCode.OK, _response.StatusCode);
                break;
            case 201:
                Assert.AreEqual(HttpStatusCode.Created, _response.StatusCode);
                break;
            case 400:
                Assert.AreEqual(HttpStatusCode.BadRequest, _response.StatusCode);
                break;
            case 401:
                Assert.AreEqual(HttpStatusCode.Unauthorized, _response.StatusCode);
                break;
            case 403:
                Assert.AreEqual(HttpStatusCode.Forbidden, _response.StatusCode);
                break;
        }
    }

    [Then(@"the user attempts to retrieve a list of created FileSends for device with id (.*)")]
    public async Task ThenTheUserShouldBeAbleToRetrieveAListOfCreatedFileSendsForDeviceWithId(int deviceId)
    {
        _response = await _client.GetAsync(
            $"/orchestrate/device-firmware/Firmware/{deviceId}"
        );

        var content = await _response.Content.ReadFromJsonAsync<IEnumerable<FileSendResponseDTO>>();
        retrievedFileSends = content.ToList();
    }

    [Then(@"the response should contain a list of created FileSends")]
    public void ThenTheResponseShouldContainAListOfCreatedFileSends()
    {
        var retrievedList = retrievedFileSends?.ToList();

        Assert.That(retrievedList?.Count, Is.EqualTo(createdFileSends.Count));
    }
}