using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("device-settings")]
    [ApiController]
    public class DeviceSettingsController : ControllerBase
    {
        private readonly IDeviceSettingsService _deviceSettingsService;

        public DeviceSettingsController(IDeviceSettingsService deviceSettingsService)
        {
            _deviceSettingsService = deviceSettingsService;
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<DeviceSettingsResponseDTO>> GetDeviceSettingsAsync(int deviceId)
        {
            try
            {
                var deviceSettingsForDevice = await _deviceSettingsService.GetSettingsForDeviceAsync(deviceId);

                return Ok(deviceSettingsForDevice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}