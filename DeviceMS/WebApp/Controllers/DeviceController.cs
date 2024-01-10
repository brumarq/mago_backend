using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.Device;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.Exceptions;

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

        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<DeviceResponseDTO>> CreateDeviceAsync([FromBody] CreateDeviceDTO createDeviceDto)
        {
            try
            {
                var newDevice = await _deviceService.CreateDeviceAsync(createDeviceDto);

                return (newDevice == null)
                    ? StatusCode(500, "The Device could not be created.")
                    : CreatedAtAction(nameof(GetDeviceByIdAsync), new { id = newDevice.Id }, newDevice);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet]
        [Authorize("Admin")]
        public async Task<ActionResult<IEnumerable<DeviceResponseDTO>>> GetDevicesAsync()
        {
            try
            {
                var devices = await _deviceService.GetDevicesAsync();
                return Ok(devices);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize("All")]
        public async Task<ActionResult<DeviceResponseDTO>> GetDeviceByIdAsync(int id)
        {
            var loggedUserId = _authenticationService.GetUserId();

            if (!await _authorizationService.IsDeviceAccessibleToUser(loggedUserId, id))
            {
                return Unauthorized($"The logged user cannot access this device.");
            }

            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                return (device == null) ? NotFound("The selected device does not exist") : Ok(device);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

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