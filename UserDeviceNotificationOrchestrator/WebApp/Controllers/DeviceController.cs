using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("orchestrator/device/user-on-device")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Assign user to a device. Accessible by Admin.
        /// </summary>
        /// <param name="createUserOnDeviceDTO">Body of request</param>
        /// <returns>Returns the new relationship.</returns>
        /// <response code="200">Returns the new relationship.</response>
        /// <response code="404">User or Device not found.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<UserOnDeviceResponseDTO>> CreateUserOnDeviceEntryAsync([FromBody] CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            try
            {
                var notificationResponseDTO = await _deviceService.CreateUserOnDeviceEntryAsync(createUserOnDeviceDTO);
                return Ok(notificationResponseDTO);
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
        /// Remove user from a device. Accessible by Admin.
        /// </summary>
        /// <param name="userId">The ID of the user to remove</param>
        /// <param name="deviceId">The ID of the device to remove from</param>
        /// <returns>Return OK</returns>
        /// <response code="200">Returns OK.</response>
        /// <response code="404">User or Device not found.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{userId}/{deviceId}")]
        [Authorize("Admin")]
        public async Task<ActionResult> DeleteUserOnDeviceEntryAsync(string userId, int deviceId)
        {
            try
            {
                ValidatePositiveNumber(deviceId, nameof(deviceId));
                await _deviceService.DeleteUserOnDeviceEntryAsync(userId, deviceId);
                return Ok();
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

        private void ValidatePositiveNumber(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new BadRequestException($"The {parameterName} cannot be negative or 0.");
            }
        }
    }
}
