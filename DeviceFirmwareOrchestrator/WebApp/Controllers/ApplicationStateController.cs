using Application.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Middleware.Prometheus;

namespace WebApp.Controllers;

[ApiController]
public class ApplicationStateController : ControllerBase
{
    private readonly IApplicationStateService _applicationStateService;
    private readonly CustomMetrics _customMetrics;

    public ApplicationStateController(IApplicationStateService applicationStateService,
        CustomMetrics customMetrics)
    {
        _applicationStateService = applicationStateService;
        _customMetrics = customMetrics;
    }

    [HttpGet("/health")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<bool>> HealthCheck()
    {
        // Checks whether application is up and running so OK is immediately returned
        _customMetrics.HealthCheckGauge.Set(1);
        return Ok();
    }

    [HttpGet("/ready")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ReadyCheck()
    {
        if (await _applicationStateService.MicroservicesReady())
        {
            _customMetrics.ReadinessCheckGauge.Set(1);
            return Ok("status: ready");
        }

        _customMetrics.ReadinessCheckGauge.Set(0);
        return StatusCode(503, "status: Dependent microservice not ready to handle traffic.");
    }
}