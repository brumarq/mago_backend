using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("orchestrate/device-metrics/[controller]")]
[ApiController]
public class DeviceMetricsController : ControllerBase
{
    private readonly IDeviceMetricsService _service;

    public DeviceMetricsController(IDeviceMetricsService service)
    {
        _service = service;
    }

    [HttpGet("{deviceId}")]
    public async Task<ActionResult<IEnumerable<DeviceMetricsResponseDTO>>> GetDeviceMetrics(int deviceId)
    {
        try
        {
            var deviceMetrics = await _service.GetDeviceMetricsAsync(deviceId);

            return Ok(deviceMetrics);
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


    [HttpGet("{aggregatedLogDateType}/{deviceId}/{fieldId}")]
    public async Task<ActionResult<IEnumerable<DeviceAggregatedLogsResponseDTO>>> GetDeviceAggregatedLogs(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
    {
        try
        {
            var deviceAggregatedLogs = await _service.GetDeviceAggregatedLogsAsync(aggregatedLogDateType, deviceId, fieldId);

            return Ok(deviceAggregatedLogs);
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