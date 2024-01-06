using Application.ApplicationServices.Interfaces;
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

        // GET: /notifications
        [HttpGet]
        [Authorize("Admin")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetAllNotificationsAsync()
        {
            try
            {
                var notifications = await _notificationService.GetAllNotificationsAsync();
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

        // GET /notifications/5
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize("All")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationByIdAsync(int id)
        {   
            try
            {
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

        // GET /notifications/device/5
        [HttpGet("device/{deviceId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId)
        {
            try
            {
                if (!IsRequestFromOrchestrator(HttpContext.Request))
                {
                    return Unauthorized("Access denied");
                }
                
                var notificationDTO = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId);
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

        // POST /<notifications>
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

        [HttpGet("statusType/{id}")]
        [Authorize("Admin")]
        public async Task<ActionResult<StatusTypeDTO>> GetStatusTypeByIdAsync(int id)
        {
            try
            {
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
        
        // DELETE /notification/statusType/5
        [HttpDelete("statusType/{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteStatusTypeAsync(int id)
        {
            try
            {
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
        
        // PUT /notification/statusType
        [HttpPut("statusType/{id}")]
        [Authorize("Admin")]
        public async Task<ActionResult<StatusTypeDTO>> UpdateStatusTypeAsync(int id, [FromBody] CreateStatusTypeDTO statusTypeDTO)
        {
            try
            {
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
