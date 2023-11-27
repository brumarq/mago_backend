using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.DeviceType;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceTypeController : ControllerBase
    {
        private readonly IDeviceTypeService _deviceTypeService;

        public DeviceTypeController(IDeviceTypeService deviceTypeService)
        {
            _deviceTypeService = deviceTypeService;
        }

        [HttpPost]
        public async Task<ActionResult<DeviceTypeResponseDTO>> CreateDeviceTypeAsync([FromBody] CreateDeviceTypeDTO createDeviceTypeDto)
        {
            try
            {
                var newDeviceType = await _deviceTypeService.CreateDeviceTypeAsync(createDeviceTypeDto);

                return (newDeviceType == null)
                    ? StatusCode(500, "The DeviceType could not be created.")
                    : Ok(newDeviceType);
                //TODO: replace OK with CreatedAtAction(nameof(GetDeviceTypeByIdAsync), new { id = newDeviceType.Id }, newDeviceType);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceTypeResponseDTO>>> GetDeviceTypesAsync()
        {
            try
            {
                var deviceTypes = await _deviceTypeService.GetDeviceTypesAsync();
                return Ok(deviceTypes);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceTypeResponseDTO>> GetDeviceTypeByIdAsync(int id)
        {
            // if (id <= 0)
            //     return BadRequest();
            
            try
            {
                var deviceType = await _deviceTypeService.GetDeviceTypeByIdAsync(id);
                return (deviceType == null) ? NotFound() : Ok(deviceType);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateDeviceTypeDTO>> UpdateDeviceTypeAsync(int id, [FromBody] UpdateDeviceTypeDTO updateDeviceTypeDto)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");
                
            try
            {
                var isUpdatedResult = await _deviceTypeService.UpdateDeviceTypeAsync(id, updateDeviceTypeDto);

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