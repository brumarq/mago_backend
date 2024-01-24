using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceType;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// DeviceType controller
    /// </summary>
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceTypeController : ControllerBase
    {
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;

        /// <summary>
        /// DeviceType controller constructor
        /// </summary>
        /// <param name="deviceTypeService"></param>
        /// <param name="authenticationService"></param>
        /// <param name="authorizationService"></param>
        public DeviceTypeController(IDeviceTypeService deviceTypeService, IAuthenticationService authenticationService, IAuthorizationsService authorizationService)
        {
            _deviceTypeService = deviceTypeService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Creates a new device type. Accessible by Admin.
        /// </summary>
        /// <param name="createDeviceTypeDto">The data transfer object for creating a new device type.</param>
        /// <returns>Returns the created device type.</returns>
        /// <response code="200">Returns the newly created device type.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<DeviceTypeResponseDTO>> CreateDeviceTypeAsync([FromBody] CreateDeviceTypeDTO createDeviceTypeDto)
        {
            try
            {
                var newDeviceType = await _deviceTypeService.CreateDeviceTypeAsync(createDeviceTypeDto);

                return (newDeviceType == null)
                    ? StatusCode(500, "The DeviceType could not be created.")
                    : Created("", newDeviceType);
            }
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
            
        }
        
        /// <summary>
        /// Retrieves all device types. Accessible by Admin.
        /// </summary>
        /// <returns>Returns a list of all device types.</returns>
        /// <response code="200">Returns a list of device types.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Authorize("Admin")]
        public async Task<ActionResult<IEnumerable<DeviceTypeResponseDTO>>> GetDeviceTypesAsync([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = null)
        {
            try
            {
                var deviceTypes = await _deviceTypeService.GetDeviceTypesAsync(pageNumber,  pageSize);
                return Ok(deviceTypes);
            }
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
        
        /// <summary>
        /// Retrieves a specific device type by ID. Accessible by Admin.
        /// </summary>
        /// <param name="id">The ID of the device type to retrieve.</param>
        /// <returns>Returns the requested device type.</returns>
        /// <response code="200">Returns the requested device type.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Device type not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [Authorize("Admin")]
        public async Task<ActionResult<DeviceTypeResponseDTO>> GetDeviceTypeByIdAsync(int id)
        {        
            try
            {
                var deviceType = await _deviceTypeService.GetDeviceTypeByIdAsync(id);
                return (deviceType == null) ? NotFound() : Ok(deviceType);
            }
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        /// <summary>
        /// Updates a specific device type. Accessible by Admin.
        /// </summary>
        /// <param name="id">The ID of the device type to update.</param>
        /// <param name="updateDeviceTypeDto">The data transfer object for updating a device type.</param>
        /// <returns>Returns the result of the update operation.</returns>
        /// <response code="204">No content if the device type was updated successfully.</response>
        /// <response code="400">Bad request if the ID is invalid.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Device type not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [Authorize("Admin")]
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
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}