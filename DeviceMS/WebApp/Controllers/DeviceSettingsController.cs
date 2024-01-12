using Application.ApplicationServices.Interfaces;
using Application.DTOs.Setting;
using Application.DTOs.SettingValue;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class DeviceSettingsController : ControllerBase
    {
        private readonly IDeviceSettingsService _deviceSettingsService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;

        public DeviceSettingsController(IDeviceSettingsService deviceSettingsService, IAuthenticationService authenticationService, IAuthorizationsService authorizationService)
        {
            _deviceSettingsService = deviceSettingsService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }

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
                    : Ok(newSetting);
                //TODO: replace OK with CreatedAtAction(nameof(GetSettingById), new { id = newSetting.Id }, newSetting);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet("{deviceId}")]
        [Authorize("All")]
        public async Task<ActionResult<IEnumerable<SettingValueResponseDTO>>> GetDeviceSettingsAsync(int deviceId)
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
                var deviceSettingsForDevice = await _deviceSettingsService.GetSettingsForDeviceAsync(deviceId);

                return Ok(deviceSettingsForDevice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult<UpdateSettingValueDTO>> UpdateSettingAsync(int id,
        //     [FromBody] UpdateSettingValueDTO updateSettingValueDto)
        // {
        //     if (id <= 0)
        //         return BadRequest("Invalid ID");
        //
        //     try
        //     {
        //         var isUpdatedResult = await _deviceSettingsService.UpdateSettingAsync(id, updateSettingValueDto);
        //
        //         if (isUpdatedResult == null)
        //             return NotFound();
        //
        //         return (bool)isUpdatedResult ? NoContent() : Ok("No changes were made.");
        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(500, $"Internal server error: {e.Message}");
        //     }
        // }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteSettingFromDeviceAsync(int id)
        // {
        //     if (id <= 0)
        //         return BadRequest("Invalid ID");
        //
        //     try
        //     {
        //  
        //         var isDeletedResult = await _deviceSettingsService.DeleteSettingFromDeviceAsync(id);
        //         
        //         if (!isDeletedResult)
        //             return NotFound();
        //
        //         return NoContent();
        //
        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(500, $"Internal server error: {e.Message}");
        //     }
        // }
    }
}