using Application.ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("metrics")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsService _metricsService;

        public MetricsController(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<LogCollection>> GetDeviceMetrics(int deviceId)
        {
            try
            {
                var deviceMetrics = await _metricsService.GetDeviceMetricsAsync(deviceId);

                return Ok(deviceMetrics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("aggregated-logs/{aggregatedLogDateType}")]
        public async Task<ActionResult<AggregatedLog>> GetAggregatedLogs(AggregatedLogDateType aggregatedLogDateType)
        {
            try
            {
                var aggregatedLogs = await _metricsService.GetAggregatedLogsAsync(aggregatedLogDateType);

                return Ok(aggregatedLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}