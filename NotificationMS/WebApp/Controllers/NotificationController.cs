using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: /notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetAllNotificationsAsync()
        {
            try
            {
                var notifications = await _notificationService.GetAllNotificationsAsync();
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET /notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationByIdAsync(int id)
        {
            try
            {
                var notificationDTO = await _notificationService.GetNotificationByIdAsync(id);
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

        // GET /notifications/device/5
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId)
        {
            try
            {
                var notificationDTO = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId);
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

        // POST /<notifications>
        [HttpPost]
        public async Task<ActionResult<NotificationResponseDTO>> CreateNotificationAsync([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            try
            {
                var result = await _notificationService.CreateNotificationAsync(createNotificationDTO);
                if (result == null)
                {
                    return StatusCode(500, "The notification could not be created.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("statusType/{id}")]
        public async Task<ActionResult<StatusTypeDTO>> GetStatusTypeByIdAsync(int statusTypeId)
        {
            try
            {
                var statusTypeDTO = await _notificationService.GetStatusTypeByIdAsync(statusTypeId);
                if (statusTypeDTO == null)
                {
                    return NotFound();
                }

                return Ok(statusTypeDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST /notification/statusType
        [HttpPost("statusType")]
        public async Task<ActionResult<StatusTypeDTO>> CreateStatusTypeAsync([FromBody] CreateStatusTypeDTO statusTypeDTO)
        {
            try
            {
                await _notificationService.CreateStatusTypeAsync(statusTypeDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // DELETE /notification/statusType/5
        [HttpDelete("statusType/{id}")]
        public async Task<IActionResult> DeleteStatusTypeAsync(int id)
        {
            try
            {
                await _notificationService.DeleteStatusTypeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // PUT /notification/statusType
        [HttpPut("statusType/{id}")]
        public async Task<ActionResult<StatusTypeDTO>> UpdateStatusTypeAsync(int id, [FromBody] CreateStatusTypeDTO statusTypeDTO)
        {
            try
            {
                var result = await _notificationService.UpdateStatusTypeAsync(id, statusTypeDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
