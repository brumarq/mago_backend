using Application.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Middleware.prometheus;
using WebApp.Middleware.status;

namespace WebApp.Controllers;

[ApiController]
public class ApplicationStateController : ControllerBase
{
    private readonly IApplicationStateService _applicationStateService;
    private readonly MigrationStatus _migrationStatus;
    private readonly CustomMetrics _customMetrics;

    public ApplicationStateController(IApplicationStateService applicationStateService, MigrationStatus migrationStatus,
        CustomMetrics customMetrics)
    {
        _applicationStateService = applicationStateService;
        _migrationStatus = migrationStatus;
        _customMetrics = customMetrics;
    }

    [HttpGet("/health")]
    public async Task<ActionResult<bool>> HealthCheck()
    {
        // Checks whether application is up and running so OK is immediately returned
        _customMetrics.HealthCheckGauge.Set(1);
        return Ok();
    }

    [HttpGet("/ready")]
    public async Task<IActionResult> ReadyCheck()
    {
        if (await _applicationStateService.DbIsConnected())
        {
            if (_migrationStatus.IsMigrationSuccessful)
            {
                _customMetrics.ReadinessCheckGauge.Set(1);
                return Ok("status: ready");
            }

            _customMetrics.ReadinessCheckGauge.Set(0);
            return StatusCode(503, "status: failed to apply pending database migrations.");
        }

        _customMetrics.ReadinessCheckGauge.Set(0);
        return StatusCode(503, "status: could not connect to database");
    }
}