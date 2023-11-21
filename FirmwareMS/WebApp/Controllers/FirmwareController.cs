using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Domain.Entities;

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
        public async Task<ActionResult<CreateFileSendDTO>> CreateFirmWareFileSend([FromBody] CreateFileSendDTO createFileSendDTO)
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

        // GET: /firmware/device/{id}
        [HttpGet("firmware/devices/{id}")]
        public async Task<ActionResult<IEnumerable<FileSendResponseDTO>>> GetFirmwareHistoryForDevice(int id)
        {
            try
            {
                var notificationDTO = await _firmwareService.GetFileSendHistoryByDeviceId(id);
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
