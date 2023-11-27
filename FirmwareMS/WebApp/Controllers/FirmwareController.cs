using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("firmware")]
    [ApiController]
    public class FirmwareController : ControllerBase
    {
        private readonly IFirmwareService _firmwareService;

        public FirmwareController(IFirmwareService firmwareService)
        {
            _firmwareService = firmwareService;
        }

        // POST /firmware
        [HttpPost]
        public async Task<ActionResult<CreateFileSendDTO>> CreateFirmWareFileSendAsync([FromBody] CreateFileSendDTO createFileSendDTO)
        {
            try
            {
                var result = await _firmwareService.CreateFileSendAsync(createFileSendDTO);
                if (result == null)
                {
                    return StatusCode(500, "The filesend could not be created.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: /firmware/device/{deviceId}
        [HttpGet("firmware/devices/{deviceId}")]
        public async Task<ActionResult<IEnumerable<FileSendResponseDTO>>> GetFirmwareHistoryForDeviceAsync(int deviceId)
        {
            try
            {
                var notificationDTO = await _firmwareService.GetFileSendHistoryByDeviceIdAsync(deviceId);
                if (notificationDTO == null)
                {
                    return NotFound();
                }

                return Ok(notificationDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
