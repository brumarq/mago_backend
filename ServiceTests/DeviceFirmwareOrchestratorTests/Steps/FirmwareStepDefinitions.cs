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
    
    private static IConfiguration _configuration;

    [BeforeTestRun]
    public static void InitializeConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
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
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["AdminJWTToken"]);
    }

    [When(@"the user creates multiple FileSends for an existing device with id (.*)")]
    public async Task WhenTheUserCreatesMultipleFileSendsForAnExistingDeviceWithId(int deviceId)
    {
        // Attempt to create 3 Firmware FileSends
        for (int i = 0; i < 3; i++)
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
            Assert.AreEqual(HttpStatusCode.OK, _response.StatusCode);

            var createdFileSend = await _response.Content.ReadFromJsonAsync<FileSendResponseDTO>();
            if (createdFileSend != null) createdFileSends.Add(createdFileSend);
        }
    }

    [Then(@"the user should be able to retrieve a list of created FileSends for device with id (.*)")]
    public async Task ThenTheUserShouldBeAbleToRetrieveAListOfCreatedFileSendsForDeviceWithId(int deviceId)
    {
        _response = await _client.GetAsync(
            $"/orchestrate/device-firmware/Firmware/{deviceId}"
        );
        Assert.AreEqual(HttpStatusCode.OK, _response.StatusCode);

        var retrievedFileSends = await _response.Content.ReadFromJsonAsync<IEnumerable<FileSendResponseDTO>>();
        var retrievedList = retrievedFileSends?.ToList();

        // Check if retrieved filesends match created filesends
        foreach (var fileSend in createdFileSends)
        {
            Assert.Contains(fileSend, retrievedList);
        }
    }
}