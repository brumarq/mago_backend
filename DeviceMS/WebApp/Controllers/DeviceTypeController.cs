using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("device-types")]
    [ApiController]
    public class DeviceTypeController : ControllerBase
    {
        private readonly IDeviceTypeService _deviceTypeService;

        public DeviceTypeController(IDeviceTypeService deviceTypeService)
        {
            _deviceTypeService = deviceTypeService;
        }

        [HttpGet("")]
        public async Task<ActionResult<DeviceTypeResponseDTO>> GetDeviceTypesAsync()
        {
            try
            {
                var deviceTypes = await _deviceTypeService.GetDeviceTypesAsync();

                return Ok(deviceTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("")]
        public async Task<ActionResult<CreateDeviceTypeDTO>> CreateDeviceTypeAsync([FromBody] CreateDeviceTypeDTO createDeviceTypeDTO)
        {
            try
            {
                var deviceType = await _deviceTypeService.CreateDeviceTypeAsync(createDeviceTypeDTO);

                return Ok(deviceType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateDeviceTypeDTO>> GetDeviceTypesAsync(int id, [FromBody] UpdateDeviceTypeDTO updateDeviceTypeDTO)
        {
            try
            {
                var deviceType = await _deviceTypeService.UpdateDeviceTypeAsync(id, updateDeviceTypeDTO);

                return Ok(deviceType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}