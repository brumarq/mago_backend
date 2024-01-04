using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Application.ApplicationServices.Interfaces.IAuthorizationService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("orchestrator/notification")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthenticationService _authenticationService;

        public NotificationsController(INotificationService notificationService, IAuthorizationService authorizationService, IAuthenticationService authenticationService)
        {
            this._notificationService = notificationService;
            _authorizationService = authorizationService;
            _authenticationService = authenticationService;
        }


        [HttpGet("device/{deviceId}")]
        [Authorize("All")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId)
        {
            try
            {
                var notificationDTOs = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId);
                return Ok(notificationDTOs);
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

        // GET /notifications/5
        [HttpGet("{id}")]
        [Authorize("All")]
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
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<NotificationResponseDTO>> CreateNotificationAsync([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            try
            {
                var notificationResponseDTO = await _notificationService.CreateNotificationAsync(createNotificationDTO);
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



    }
}
