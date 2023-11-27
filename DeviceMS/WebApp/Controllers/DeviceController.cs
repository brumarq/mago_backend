using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.Device;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost]
        public async Task<ActionResult<DeviceResponseDTO>> CreateDeviceAsync([FromBody] CreateDeviceDTO createDeviceDto)
        {
            try
            {
                var newDevice = await _deviceService.CreateDeviceAsync(createDeviceDto);

                return (newDevice == null)
                    ? StatusCode(500, "The Device could not be created")
                    : Ok(newDevice);
                //TODO: replace OK with CreatedAtAction(nameof(GetDeviceByIdAsync), new { id = newDevice.Id }, newDevice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResponseDTO>>> GetDevicesAsync()
        {
            try
            {
                var devices = await _deviceService.GetDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceResponseDTO>> GetDeviceByIdAsync(int id)
        {
            // if (id <= 0)
            //     return BadRequest();
            
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                return (device == null) ? NotFound() : Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateDeviceDTO>> UpdateDeviceAsync(int id,
            [FromBody] UpdateDeviceDTO updateDeviceDto)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                var isUpdatedResult = await _deviceService.UpdateDeviceAsync(id, updateDeviceDto);

                if (isUpdatedResult == null)
                    return NotFound();

                return (bool)isUpdatedResult ? NoContent() : Ok("No changes were made.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}