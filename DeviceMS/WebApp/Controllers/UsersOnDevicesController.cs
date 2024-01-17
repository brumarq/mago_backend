using Application.ApplicationServices.Interfaces;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    /// <summary>
    /// UserOnDevices controller
    /// </summary>
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class UsersOnDevicesController : ControllerBase
    {
        private readonly ILogger<UsersOnDevicesController> _logger;

        private readonly IUsersOnDevicesService _usersOnDevicesService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;
        private readonly string? _orchestratorApiKey;

        /// <summary>
        /// UserOnDevices controller constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="usersOnDevicesService"></param>
        /// <param name="authenticationService"></param>
        /// <param name="authorizationService"></param>
        /// <param name="logger"></param>
        public UsersOnDevicesController(IConfiguration configuration, IUsersOnDevicesService usersOnDevicesService, IAuthenticationService authenticationService, IAuthorizationsService authorizationService, ILogger<UsersOnDevicesController> logger)
        {
            _usersOnDevicesService = usersOnDevicesService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
            _orchestratorApiKey = configuration["OrchestratorApiKey"];

        }
        
        /// <summary>
        /// Retrieves users associated with devices for a given user ID. Accessible by all authorized users.
        /// </summary>
        /// <param name="userId">The user ID for which to retrieve device associations.</param>
        /// <returns>Returns a list of users associated with devices.</returns>
        /// <response code="200">Returns a list of users on devices.</response>
        /// <response code="401">Unauthorized access if the logged-in user does not match the requested user ID or lacks required permissions.</response>
        /// <response code="500">Internal server error.</response>
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
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
        
        /// <summary>
        /// Creates a new user-device association. Accessible by Admin.
        /// </summary>
        /// <param name="createUserOnDeviceDTO">The data transfer object for creating a new user on device entry.</param>
        /// <returns>Returns the newly created user-device association.</returns>
        /// <response code="200">Returns the newly created user-device association.</response>
        /// <response code="401">Unauthorized access if the request does not come from an authorized orchestrator.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize("Admin")]
        [ApiExplorerSettings(IgnoreApi = true)]
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
                    : Created("", newUserOnDeviceEntry);
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

        /// <summary>
        /// Deletes a user-device association. Accessible by Admin.
        /// </summary>
        /// <param name="id">The ID of the user-device association to delete.</param>
        /// <returns>Returns the result of the deletion operation.</returns>
        /// <response code="204">No content if the user-device association was successfully deleted.</response>
        /// <response code="404">Not found if the user-device association with the specified ID does not exist.</response>
        /// <response code="500">Internal server error.</response>
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
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
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
