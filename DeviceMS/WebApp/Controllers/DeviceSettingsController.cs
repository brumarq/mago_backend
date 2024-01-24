using Application.ApplicationServices.Interfaces;
using Application.DTOs.Setting;
using Application.DTOs.SettingValue;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// DeviceSettings controller
    /// </summary>
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceSettingsController : ControllerBase
    {
        private readonly IDeviceSettingsService _deviceSettingsService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;

        /// <summary>
        /// DeviceSettings controller constructor
        /// </summary>
        /// <param name="deviceSettingsService"></param>
        /// <param name="authenticationService"></param>
        /// <param name="authorizationService"></param>
        public DeviceSettingsController(IDeviceSettingsService deviceSettingsService, IAuthenticationService authenticationService, IAuthorizationsService authorizationService)
        {
            _deviceSettingsService = deviceSettingsService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Adds a new setting to a device. Accessible by all authorized users.
        /// </summary>
        /// <param name="newSettingDto">The data transfer object for creating a new setting.</param>
        /// <returns>Returns the added setting.</returns>
        /// <response code="200">Returns the newly added setting.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access if the user cannot access the specified device.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize("All")]
        public async Task<ActionResult<SettingValueResponseDTO>> AddSettingToDeviceAsync(
            CreateSettingValueDTO newSettingDto)
        {
            
            var loggedUserId = _authenticationService.GetUserId();

            if (!await _authorizationService.IsDeviceAccessibleToUser(loggedUserId, newSettingDto.DeviceId))
            {
                return StatusCode(403, $"The logged in user cannot access device with id {newSettingDto.DeviceId}");
            }
            
            try
            {
                var newSetting = await _deviceSettingsService.AddSettingToDevice(newSettingDto);

                return (newSetting == null)
                    ? StatusCode(500, "The Setting could not be added to the device.")
                    : Created("", newSetting);
            }
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        /// <summary>
        /// Retrieves settings for a specific device. Accessible by all authorized users.
        /// </summary>
        /// <param name="deviceId">The ID of the device whose settings are to be retrieved.</param>
        /// <returns>Returns a list of settings for the specified device.</returns>
        /// <response code="200">Returns a list of device settings.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access if the user cannot access the specified device.</response>
        /// <response code="400">Bad request if the device ID is invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{deviceId}")]
        [Authorize("All")]
        public async Task<ActionResult<IEnumerable<SettingValueResponseDTO>>> GetDeviceSettingsAsync(int deviceId, [FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {  
            var loggedUserId = _authenticationService.GetUserId();

            if (!await _authorizationService.IsDeviceAccessibleToUser(loggedUserId, deviceId))
            {
                return StatusCode(403, $"The logged in user cannot access device with id {deviceId}");
            }
            
            if (deviceId <= 0)
                throw new BadRequestException("Invalid ID");

            try
            {
                var deviceSettingsForDevice = await _deviceSettingsService.GetSettingsForDeviceAsync(deviceId, pageNumber, pageSize);

                return Ok(deviceSettingsForDevice);
            }
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}