using Application.ApplicationServices.Interfaces;
using Application.DTOs.Firmware;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("orchestrate/device-firmware/[controller]")]
[ApiController]
public class FirmwareController : ControllerBase
{
    private readonly IFirmwareService _service;

    public FirmwareController(IFirmwareService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize("Admin")]
    public async Task<ActionResult<FileSendResponseDTO>> CreateFirmwareFileSend(
        [FromBody] CreateFileSendDTO newFileSendDto)
    {
        try
        {
            var newFileSend = await _service.CreateFileSendAsync(newFileSendDto);

            return (newFileSend == null)
                ? StatusCode(500, "The firmware file could not be created.")
                : Created("", newFileSend);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    [HttpGet("{deviceId}")]
    [Authorize("Admin")]
    public async Task<ActionResult<IEnumerable<FileSendResponseDTO>>> GetFirmwareHistoryForDevice(int deviceId)
    {
        try
        {
            var fileSends = await _service.GetFirmwareHistoryForDeviceAsync(deviceId);
            return Ok(fileSends);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }
}