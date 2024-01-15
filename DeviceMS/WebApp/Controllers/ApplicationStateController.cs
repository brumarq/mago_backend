using Application.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Middleware.status;

namespace WebApp.Controllers;

[ApiController]
public class ApplicationStateController : ControllerBase
{
    private readonly IApplicationStateService _applicationStateService;
    private readonly MigrationStatus _migrationStatus;

    public ApplicationStateController(IApplicationStateService applicationStateService, MigrationStatus migrationStatus)
    {
        _applicationStateService = applicationStateService;
        _migrationStatus = migrationStatus;
    }

    [HttpGet("/health")]
    public async Task<ActionResult<bool>> HealthCheck()
    {
        // Checks whether application is up and running so OK is immediately returned
        return Ok();
    }

    [HttpGet("/ready")]
    public async Task<IActionResult> ReadyCheck()
    {
        if (await _applicationStateService.DbIsConnected())
        {
            return _migrationStatus.IsMigrationSuccessful
                ? Ok("status: ready")
                : StatusCode(503, "status: failed to apply pending database migrations.");
        }

        return StatusCode(503, "status: could not connect to database");
    }
}