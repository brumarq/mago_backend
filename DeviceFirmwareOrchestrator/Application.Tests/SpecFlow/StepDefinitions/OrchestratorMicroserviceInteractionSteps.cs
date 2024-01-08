using System.Net;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Firmware;
using TechTalk.SpecFlow;

namespace Application.Tests.StepDefinitions;

[Binding]
public class OrchestratorMicroserviceInteractionSteps
{
    private readonly IFirmwareService _firmwareService;
    private FileSendResponseDTO _fileSendResponse;
    private HttpRequestException _caughtException;


    [Given(@"a device with ID (.*) exists")]
    public void GivenADeviceWithIdExists(int deviceId)
    {
        
    }

    [When(@"a new firmware send request is created for device (.*)")]
    public async void WhenANewFirmwareSendRequestIsCreatedForDevice(int deviceId)
    {
        try
        {
            var newFileSendDto = new CreateFileSendDTO { DeviceId = deviceId /* other properties */ };
            _fileSendResponse = await _firmwareService.CreateFileSendAsync(newFileSendDto);
        }
        catch (HttpRequestException e)
        {
            _caughtException = e;
        }
    }

    [Then(@"the post firmware FileSend request should be processed successfully with status code (.*)")]
    public void ThenThePostFirmwareFileSendRequestShouldBeProcessedSuccessfullyWithStatusCode(int statusCode)
    {
        Assert.IsNotNull(_fileSendResponse);
        Assert.That((int)_caughtException.StatusCode!, Is.EqualTo(statusCode), $"Expected a status code of {statusCode}, but got {(int)_caughtException.StatusCode}.");
    }

    [Given(@"a device with ID (.*) does not exist")]
    public void GivenADeviceWithIdDoesNotExist(int deviceId)
    {
        
    }

    [Then(@"a NotFoundException should be thrown")]
    public void ThenANotFoundExceptionShouldBeThrown()
    {
        Assert.That(_caughtException.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}