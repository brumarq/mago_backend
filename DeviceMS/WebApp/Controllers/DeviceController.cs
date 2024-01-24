using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;


        public DeviceController(IDeviceService deviceService, IAuthenticationService authenticationService,
            IAuthorizationsService authorizationService)
        {
            _deviceService = deviceService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        
        /// <summary>
        /// Creates a new device. Accessible by Admin.
        /// </summary>
        /// <param name="createDeviceDto">The data transfer object for device creation.</param>
        /// <returns>Returns the created device.</returns>
        /// <response code="201">Returns the newly created device.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<DeviceResponseDTO>> CreateDeviceAsync([FromBody] CreateDeviceDTO createDeviceDto)
        {
            try
            {
                var newDevice = await _deviceService.CreateDeviceAsync(createDeviceDto);

                return (newDevice == null)
                    ? StatusCode(500, "The Device could not be created.")
                    : Created("", newDevice);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
        
        /// <summary>
        /// Retrieves all devices. Accessible by Admin.
        /// </summary>
        /// <returns>Returns a list of devices.</returns>
        /// <response code="200">Returns a list of devices.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>

        [HttpGet]
        [Authorize("Admin")]
        public async Task<ActionResult<IEnumerable<DeviceResponseDTO>>> GetDevicesAsync([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = null)
        {
            try
            {
                var devices = await _deviceService.GetDevicesAsync(pageNumber, pageSize);
                return Ok(devices);
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
        /// Retrieves a specific device by ID. Accessible by all authorized users.
        /// </summary>
        /// <param name="id">The ID of the device to retrieve.</param>
        /// <returns>Returns the requested device.</returns>
        /// <response code="200">Returns the requested device.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access if the user cannot access the specified device.</response>
        /// <response code="404">Device not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [Authorize("All")]
        public async Task<ActionResult<DeviceResponseDTO>> GetDeviceByIdAsync(int id)
        {
            var loggedUserId = _authenticationService.GetUserId();

            if (!await _authorizationService.IsDeviceAccessibleToUser(loggedUserId, id))
            {
                return StatusCode(403, $"The logged in user cannot access device with id {id}");
            }

            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                return (device == null) ? NotFound("The selected device does not exist") : Ok(device);
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
        /// Updates a specific device. Accessible by Admin.
        /// </summary>
        /// <param name="id">The ID of the device to update.</param>
        /// <param name="updateDeviceDto">The data transfer object for device update.</param>
        /// <returns>Returns the result of the update operation.</returns>
        /// <response code="204">No content if the device was updated successfully.</response>
        /// <response code="400">Bad request if the ID is invalid.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Device not found.</response>
        /// <response code="500">Internal server error.</response>

        [HttpPut("{id}")]
        [Authorize("Admin")]
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
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private bool HasPermission(string permission)
        {
            return User.HasClaim(c => c.Type == "permissions" && c.Value == permission);
        }
    }
}