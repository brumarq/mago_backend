﻿using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Application.ApplicationServices.Interfaces.IAuthorizationService;

namespace WebApp.Controllers
{
    [Route("notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly string? _orchestratorApiKey;

        public NotificationController(IConfiguration configuration, INotificationService notificationService, IAuthenticationService authenticationService, IAuthorizationService authorizationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
            _orchestratorApiKey = configuration["OrchestratorApiKey"];

        }


        /// <summary>
        /// Retrieves all notifications. Accessible by Admin.
        /// </summary>        
        /// <param name="pageNumber">The number of the page. Defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Defaults to 30.</param>
        /// <returns>Returns all notifications for all devices.</returns>
        /// <response code="200" name="NotificationResponseDTO">Returns a list of notifications.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Authorize("Admin")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetAllNotificationsAsync(int pageNumber = 1, int pageSize = 30)
        {
            try
            {
                ValidatePositiveNumber(pageNumber, nameof(pageNumber));
                ValidatePositiveNumber(pageSize, nameof(pageSize));

                var notifications = await _notificationService.GetAllNotificationsPagedAsync(pageNumber, pageSize);
                if (notifications == null || !notifications.Any())
                {
                    throw new NotFoundException("No notifications found");
                }
                return Ok(notifications);
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
        /// Retrieves notification by their id. Accessible by Admin and users on device.
        /// </summary>
        /// <returns>Returns notification for specified notification id.</returns>
        /// <response code="200" name="NotificationResponseDTO">Returns a list of notifications.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize("All")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationByIdAsync(int id)
        {   
            try
            {
                ValidatePositiveNumber(id, nameof(id));

                if (!IsRequestFromOrchestrator(HttpContext.Request))
                {
                    return Unauthorized("Access denied");
                }
                
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

        /// <summary>
        /// Retrieves notification by device id. Accessible by Admin.
        /// </summary>        
        /// <param name="deviceId">The ID of the device to notifications for.</param>
        /// <param name="pageNumber">The number of the page. Defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Defaults to 30.</param>
        /// <returns>Returns all notifications for the specified device.</returns>
        /// <response code="200" name="NotificationResponseDTO">Returns a list of notifications.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("device/{deviceId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize("All")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId, int pageNumber, int pageSize)
        {
            try
            {
                ValidatePositiveNumber(deviceId, nameof(deviceId));
                ValidatePositiveNumber(pageNumber, nameof(pageNumber));
                ValidatePositiveNumber(pageSize, nameof(pageSize));

                if (!IsRequestFromOrchestrator(HttpContext.Request))
                {
                    return Unauthorized("Access denied");
                }

                var notificationDTO = await _notificationService.GetNotificationsByDeviceIdPagedAsync(deviceId, pageNumber, pageSize);
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

        /// <summary>
        /// Creates a notification. Accessible by Admin.
        /// </summary>        
        /// <param name="createNotificationDTO">The notification to be created.</param>
        /// <returns>Returns the response DTO of the created notification.</returns>
        /// <response code="200" name="NotificationResponseDTO">Returns a list of notifications.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize("Admin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NotificationResponseDTO>> CreateNotificationAsync([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            try
            {
                if (!IsRequestFromOrchestrator(HttpContext.Request))
                {
                    return Unauthorized("Access denied");
                }
                
                var result = await _notificationService.CreateNotificationAsync(createNotificationDTO);
                if (result == null)
                {
                    return StatusCode(500, "The notification could not be created.");
                }

                return Ok(result);
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
        /// Retrieves status type by ID. Accessible by Admin.
        /// </summary>
        /// <param name="id">The ID of status type.</param>
        /// <returns>Returns status type by ID.</returns>
        /// <response code="200" name="StatusTypeDTO">Returns a status type.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("statusType/{id}")]
        [Authorize("Admin")]
        public async Task<ActionResult<StatusTypeDTO>> GetStatusTypeByIdAsync(int id)
        {
            try
            {
                ValidatePositiveNumber(id, nameof(id));
                var statusTypeDTO = await _notificationService.GetStatusTypeByIdAsync(id);
                if (statusTypeDTO == null)
                {
                    return NotFound();
                }

                return Ok(statusTypeDTO);
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
        /// Create status type. Accessible by Admin.
        /// </summary>
        /// <param name="statusTypeDTO">Body of status type to be created.</param>
        /// <returns>Returns created status type.</returns>
        /// <response code="200" name="StatusTypeDTO">Returns a status type.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        // POST /notification/statusType
        [HttpPost("statusType")]
        [Authorize("Admin")]
        public async Task<ActionResult<StatusTypeDTO>> CreateStatusTypeAsync([FromBody] CreateStatusTypeDTO statusTypeDTO)
        {
            try
            {
                await _notificationService.CreateStatusTypeAsync(statusTypeDTO);
                return Ok();
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
        /// Delete status type. Accessible by Admin.
        /// </summary>
        /// <param name="id"> Id of status type</param>
        /// <returns>Returns confirmation.</returns>
        /// <response code="204">Returns no content.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        
        // DELETE /notification/statusType/5
        [HttpDelete("statusType/{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteStatusTypeAsync(int id)
        {
            try
            {
                ValidatePositiveNumber(id, nameof(id));
                await _notificationService.DeleteStatusTypeAsync(id);
                return NoContent();
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
        /// Create status type. Accessible by Admin.
        /// </summary>
        /// <param name="id">ID of status type</param>
        /// <param name="statusTypeDTO">Body of status type to be created.</param>
        /// <returns>Returns updated status type.</returns>
        /// <response code="200" name="StatusTypeDTO">Returns a status type.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        // PUT /notification/statusType
        [HttpPut("statusType/{id}")]
        [Authorize("Admin")]
        public async Task<ActionResult<StatusTypeDTO>> UpdateStatusTypeAsync(int id, [FromBody] CreateStatusTypeDTO statusTypeDTO)
        {
            try
            {
                ValidatePositiveNumber(id, nameof(id));
                var result = await _notificationService.UpdateStatusTypeAsync(id, statusTypeDTO);
                return Ok(result);
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

        private void ValidatePositiveNumber(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new BadRequestException($"The {parameterName} cannot be negative or 0.");
            }
        }

        private bool IsRequestFromOrchestrator(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("X-Orchestrator-Key", out var receivedKey))
            {
                _logger.LogWarning("Orchestrator key header missing in request.");
                return false;
            }

            if (receivedKey != _orchestratorApiKey)
            {
                _logger.LogWarning("Invalid orchestrator key provided.");
                return false;
            }

            _logger.LogInformation("Valid orchestrator key received.");
            return true;
        }
    }
}
