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

        
        /// <summary>
        /// Get Notifications from a Device. Accessible by all users (both admin and client).
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Returns the list of notifications.</returns>
        /// <response code="200">Returns device notifications.</response>
        /// <response code="404">Device not found.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("device/{deviceId}")]
        [Authorize("All")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                ValidatePositiveNumber(deviceId, nameof(deviceId));
                ValidatePositiveNumber(pageNumber, nameof(pageNumber));
                ValidatePositiveNumber(pageSize, nameof(pageSize));

                var loggedUserId = _authenticationService.GetUserId();

                var loggedInUserId = _authenticationService.GetUserId();
                bool isUserAllowed = await _authorizationService.IsDeviceAccessibleToUser(loggedInUserId, deviceId);

                if(!isUserAllowed)
                {
                    return Unauthorized($"The logged user cannot access this device.");
                }

                var notificationDTOs = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId, pageNumber, pageSize);
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
        
        /// <summary>
        /// Get Notification by ID. Accessible by all users (both admin and client).
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Returns the notification.</returns>
        /// <response code="200">Returns notification.</response>
        /// <response code="404">Notification not found.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        // GET /notifications/5
        [HttpGet("{id}")]
        [Authorize("All")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationByIdAsync(int id)
        {
            try
            {
                ValidatePositiveNumber(id, nameof(id));
                var notificationDTO = await _notificationService.GetNotificationByIdAsync(id);
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
        
        
        /// <summary>
        /// Create notification. Accessible by Admin.
        /// </summary>
        /// <param name="createNotificationDTO">Body of notification</param>
        /// <returns>Returns the notification.</returns>
        /// <response code="200">Returns notification.</response>
        /// <response code="404">Device not found.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
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

        private void ValidatePositiveNumber(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new BadRequestException($"The {parameterName} cannot be negative or 0.");
            }
        }


    }
}
