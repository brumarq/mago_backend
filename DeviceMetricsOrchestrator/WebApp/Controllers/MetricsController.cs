using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

///<Summary>
/// DeviceMetricsController class
///</Summary>
[Route("orchestrate/device-metrics/[controller]")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly IDeviceMetricsService _deviceMetricsService;

    ///<Summary>
    /// Device metrics controller constructor
    ///</Summary>
    public MetricsController(IDeviceMetricsService deviceMetricsService)
    {
        _deviceMetricsService = deviceMetricsService;
    }

    /// <summary>
    /// Gets latest device metrics (lastest unique entry in Field for a Device) by device id | Permissions: Client and Admin
    /// </summary>
    /// <param name="deviceId">Device unique identifier</param>
    /// <returns>List of device metrics</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{deviceId}")]
    [Authorize("All")]
    public async Task<ActionResult<IEnumerable<DeviceMetricsResponseDTO>>> GetLastestMetricsForDevice(int deviceId)
    {
        try
        {
            var deviceMetrics = await _deviceMetricsService.GetLastMetricsForDeviceAsync(deviceId);

            return Ok(deviceMetrics);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }  
}