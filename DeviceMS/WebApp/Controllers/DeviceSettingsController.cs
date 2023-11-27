using Application.ApplicationServices.Interfaces;
using Application.DTOs.Setting;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceSettingsController : ControllerBase
    {
        private readonly IDeviceSettingsService _deviceSettingsService;

        public DeviceSettingsController(IDeviceSettingsService deviceSettingsService)
        {
            _deviceSettingsService = deviceSettingsService;
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<IEnumerable<SettingValueResponseDTO>>> GetDeviceSettingsAsync(int deviceId)
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

        [HttpPost]
        public async Task<ActionResult<SettingValueResponseDTO>> CreateDeviceSettingAsync(
            CreateSettingValueDTO newSetting)
        {
            try
            {
                var result = await _deviceSettingsService.AddSettingsToDevice(newSetting);
                return Created("device-settings", result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}