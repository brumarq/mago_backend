using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("orchestrator/notification")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            this._notificationService = notificationService;
        }


        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId)
        {
            try
            {
                var notificationDTOs = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId);
                return Ok(notificationDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<NotificationResponseDTO>> CreateNotificationAsync([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            try
            {
                var notificationResponseDTO = await _notificationService.CreateNotificationAsync(createNotificationDTO);
                return Ok(notificationResponseDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }



    }
}
