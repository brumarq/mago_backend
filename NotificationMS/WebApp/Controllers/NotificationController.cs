using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetAllNotifications()
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
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationById(int id)
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

        // GET /notifications/users/5
        [HttpGet("notifications/users/{id}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationForUserOnStatusType(int id)
        {
            try
            {
                var notificationDTO = await _notificationService.GetNotificationsForUserOnStatusTypeByUserId(id);
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
        [HttpGet("/notifications/device/{id}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDevice(int id)
        {
            try
            {
                var notificationDTO = await _notificationService.GetNotificationsByDeviceId(id);
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
        public async Task<ActionResult<NotificationResponseDTO>> CreateNotification([FromBody] CreateNotificationDTO createNotificationDTO)
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
    }
}
