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
[Route("orchestrate/aggregated-logs/[controller]")]
[ApiController]
public class AggregatedLogsController : ControllerBase
{
    private readonly IDeviceAggregatedLogsService _deviceAggregatedService;

    ///<Summary>
    /// Device metrics controller constructor
    ///</Summary>
    public AggregatedLogsController(IDeviceAggregatedLogsService deviceAggregatedService)
    {
        _deviceAggregatedService = deviceAggregatedService;
    }

    /// <summary>
    /// Get aggregated logs based on date type, device and field | Permissions: Client and Admin
    /// </summary>
    /// <param name="aggregatedLogDateType">Aggregation date type that refers to 'Weekly', 'Monthly', 'Yearly'</param>
    /// <param name="deviceId">Device unique identifier</param>
    /// <param name="fieldId">Field unique identifier</param>
    /// <param name="pageNumber">The page number | Defaults to 1</param>
    /// <param name="pageSize">The page size | Defaults to 50</param>
    /// <param name="startDate">Start date (optional) format: YYYY-MM-DD</param>
    /// <param name="endDate">End date (optional) format: YYYY-MM-DD</param>
    /// <returns>List of aggregated logs</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{aggregatedLogDateType}/{deviceId}/{fieldId}")]
    [Authorize("All")] 
    public async Task<ActionResult<IEnumerable<DeviceAggregatedLogsResponseDTO>>> GetAggregatedLogs(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string? startDate, string? endDate, int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            var deviceAggregatedLogs = await _deviceAggregatedService.GetDeviceAggregatedLogsAsync(aggregatedLogDateType, deviceId, fieldId, startDate, endDate, pageNumber, pageSize);

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