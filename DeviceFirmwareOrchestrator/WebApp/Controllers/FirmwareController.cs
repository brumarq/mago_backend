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

    /// <summary>
    /// Create new Firmware update. Accessible by Admin.
    /// </summary>
    /// <param name="newFileSendDto">Body of firmware update.</param>
    /// <returns>Returns firmware update.</returns>
    /// <response code="200" name="FileSendResponseDTO">Returns  firmware update.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
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

    /// <summary>
    /// Get Firmware History from a Device. Accessible by Admin.
    /// </summary>
    /// <param name="deviceId">Id of the Device</param>
    /// <returns>Returns list of firmware updates.</returns>
    /// <response code="200" name="FileSendResponseDTO">Returns list of firmware updates.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
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