using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

///<Summary>
/// DeviceMetricsController class
///</Summary>
[Route("orchestrate/device-metrics/[controller]")]
[ApiController]
public class DeviceMetricsController : ControllerBase
{
    private readonly IDeviceMetricsService _deviceMetricsService;

    ///<Summary>
    /// Device metrics controller constructor
    ///</Summary>
    public DeviceMetricsController(IDeviceMetricsService deviceMetricsService)
    {
        _deviceMetricsService = deviceMetricsService;
    }

    /// <summary>
    /// Get device metrics based on the device identifier
    /// </summary>
    /// <param name="deviceId">Device unique identifier</param>
    /// <returns>List of device metrics</returns>
    [HttpGet("{deviceId}")]
    [Authorize("All")]
    public async Task<ActionResult<IEnumerable<DeviceMetricsResponseDTO>>> GetDeviceMetrics(int deviceId)
    {
        try
        {
            var deviceMetrics = await _deviceMetricsService.GetDeviceMetricsAsync(deviceId);

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

    /// <summary>
    /// Get aggregated logs based on the aggregation date type, device id, field id (and optionally date range)
    /// </summary>
    /// <param name="aggregatedLogDateType">Aggregation date type that refers to 'Weekly', 'Monthly', 'Yearly'</param>
    /// <param name="deviceId">Device unique identifier</param>
    /// <param name="fieldId">Field unique identifier</param>
    /// <param name="startDate">Start date (optional) format: YYYY-MM-DD</param>
    /// <param name="endDate">End date (optional) format: YYYY-MM-DD</param>
    /// <returns>List of aggregated logs</returns>
    [HttpGet("{aggregatedLogDateType}/{deviceId}/{fieldId}")]
    [Authorize("All")] 
    public async Task<ActionResult<IEnumerable<DeviceAggregatedLogsResponseDTO>>> GetDeviceAggregatedLogs(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string? startDate, string? endDate)
    {
        try
        {
            var deviceAggregatedLogs = await _deviceMetricsService.GetDeviceAggregatedLogsAsync(aggregatedLogDateType, deviceId, fieldId, startDate, endDate);

            return Ok(deviceAggregatedLogs);
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