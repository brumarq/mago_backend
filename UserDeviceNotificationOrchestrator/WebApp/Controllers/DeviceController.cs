using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.UsersOnDevices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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


        [HttpPost]
        public async Task<ActionResult<UserOnDeviceResponseDTO>> CreateUserOnDeviceEntryAsync([FromBody] CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            try
            {
                var notificationResponseDTO = await _deviceService.CreateNotificationAsync(createUserOnDeviceDTO);
                return Ok(notificationResponseDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
