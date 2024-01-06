using Application.ApplicationServices.Interfaces;
using Application.DTOs.UsersOnDevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class UsersOnDevicesController : ControllerBase
    {
        private readonly ILogger<UsersOnDevicesController> _logger;

        private readonly IUsersOnDevicesService _usersOnDevicesService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;
        private readonly string? _orchestratorApiKey;

        public UsersOnDevicesController(IConfiguration configuration, IUsersOnDevicesService usersOnDevicesService, IAuthenticationService authenticationService, IAuthorizationsService authorizationService, ILogger<UsersOnDevicesController> logger)
        {
            _usersOnDevicesService = usersOnDevicesService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
            _orchestratorApiKey = configuration["OrchestratorApiKey"];

        }

        [HttpGet("{userId}")]
        [Authorize("All")]
        public async Task<ActionResult<IEnumerable<UsersOnDevicesResponseDTO>>> GetUsersOnDevicesByUserId(string userId)
        {
            var loggedUserId = _authenticationService.GetUserId();

            if (!loggedUserId.Equals(userId) && 
                _authenticationService.HasPermission("client"))
            {
                return Unauthorized($"The logged user cannot access this device.");
            }
            
            try
            {
                var usersOnDevices = await _usersOnDevicesService.GetUsersOnDevicesByUserIdAsync(userId);
                return Ok(usersOnDevices);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<UsersOnDevicesResponseDTO>> CreateUsersOnDevicesEntry([FromBody] CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            try
            {
                if (!IsRequestFromOrchestrator(HttpContext.Request))
                {
                    return Unauthorized("Access denied");
                }
                
                var newUserOnDeviceEntry = await _usersOnDevicesService.CreateUserOnDeviceAsync(createUserOnDeviceDTO);

                return (newUserOnDeviceEntry == null)
                    ? StatusCode(500, "The UserOnDevice entry could not be created.")
                    : Ok(newUserOnDeviceEntry);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleteResult = await _usersOnDevicesService.DeleteUserOnDeviceAsync(id);
                if (deleteResult)
                {
                    return NoContent(); 
                }
                else
                {
                    return NotFound($"UserOnDevice with ID {id} was not found."); 
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
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
