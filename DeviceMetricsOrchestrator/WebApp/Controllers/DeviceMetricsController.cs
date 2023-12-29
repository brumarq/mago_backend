using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [Authorize("All")]
    public async Task<ActionResult<IEnumerable<DeviceMetricsResponseDTO>>> GetDeviceMetrics(int deviceId)
    {
        try
        {
            var userId = GetUserId();

            if (!(HasPermission("client") || HasPermission("admin")))
                throw new UnauthorizedException($"The user with id {userId} does not have sufficient permissions!");

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
    [Authorize("All")]
    public async Task<ActionResult<IEnumerable<DeviceAggregatedLogsResponseDTO>>> GetDeviceAggregatedLogs(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
    {
        try
        {
            var userId = GetUserId();

            if (!(HasPermission("client") || HasPermission("admin")))
                throw new UnauthorizedException($"The user with id {userId} does not have sufficient permissions!");

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

    private string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private bool HasPermission(string permission)
    {
        return User.HasClaim(c => c.Type == "permissions" && c.Value == permission);
    }

}