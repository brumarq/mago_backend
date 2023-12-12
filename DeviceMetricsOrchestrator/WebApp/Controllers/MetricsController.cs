using Application.ApplicationServices.Interfaces;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("orchestrate/device-metrics/[controller]")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _service;

    public MetricsController(IMetricsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetricsResponseDTO>>> GetMetricsInformation(int deviceId)
    {
        try
        {
            var metricsInformation = await _service.GetMetricsForDevice(deviceId);

            return Ok(metricsInformation);
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