using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("devices")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<DeviceResponseDTO>>> GetAllDevicesAsync()
        {
            try
            {
                var devices = await _deviceService.GetAllDevicesAsync();

                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<DeviceResponseDTO>> GetDeviceById(int deviceId)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(deviceId);

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("")]
        public async Task<ActionResult<CreateDeviceDTO>> CreateDeviceAsync(CreateDeviceDTO createDeviceDTO)
        {
            try
            {
                var device = await _deviceService.CreateDeviceAsync(createDeviceDTO);

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}