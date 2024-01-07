using System.Security.Claims;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers;

[Route("users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IAuth0Service _auth0Service;
    private readonly string? _orchestratorApiKey;
    
    public UserController(IConfiguration configuration, IAuth0Service auth0Service, ILogger<UserController> logger)
    {
        _auth0Service = auth0Service;
        _logger = logger;
        _orchestratorApiKey = configuration["OrchestratorApiKey"];
    }
    /// <summary>
    /// Retrieves a user by their ID. Accessible by all users (both admin and client).
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>Returns the user details if found.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    // GET: /users/{id}
    [HttpGet("{id}")]
    [Authorize("All")]
    public async Task<ActionResult<User>> GetUserById(string id)
    {
        try
        {
            var userIdClaim = GetUserId();
            var hasAdminPermission = HasPermission("admin");
            var hasClientPermission = HasPermission("client");

            // Check if the user has the right permissions
            if (!hasAdminPermission && (!hasClientPermission || userIdClaim != id)) return Unauthorized();
            
            var userDto = await _auth0Service.GetUser(id);
                
            return Ok(userDto);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    /// <summary>
    /// Retrieves a list of all users. Accessible only by admin users.
    /// </summary>
    /// <returns>Returns a list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Authorize("Admin")]
    public async Task<ActionResult<IEnumerable<UserCompressedDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _auth0Service.GetAllUsers();
            return Ok(users);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    /// <summary>
    /// Updates a user's information by their ID. Accessible by all users (both admin and client).
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updateUserDto">The user data to be updated.</param>
    /// <returns>Returns the updated user details.</returns>
    /// <response code="200">User successfully updated.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{id}")]
    [Authorize("All")]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDto)
    {
        try
        {
            var userIdClaim = GetUserId();
            var hasAdminPermission = HasPermission("admin");
            var hasClientPermission = HasPermission("client");

            // Check if the user has the right permissions
            if (!hasAdminPermission && (!hasClientPermission || userIdClaim != id)) return Unauthorized();
            
            var updatedUser = await _auth0Service.UpdateUserAsync(id, updateUserDto);
            return Ok(updatedUser);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    /// <summary>
    /// Deletes a user by their ID. Accessible only by admin users.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>Indicates whether the deletion was successful.</returns>
    /// <response code="200">User successfully deleted.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id}")]
    [Authorize("Admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            if (!IsRequestFromOrchestrator(HttpContext.Request))
            {
                return Unauthorized("Access denied");
            }
            
            await _auth0Service.DeleteUserAsync(id);
            return Ok();
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    /// <summary>
    /// Creates a new user. Accessible only by admin users.
    /// </summary>
    /// <param name="createUserDto">The user data for the new user.</param>
    /// <returns>Returns the details of the created user.</returns>
    /// <response code="200">User successfully created.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Authorize("Admin")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO createUserDto)
    {
        try
        {
            var result = await _auth0Service.CreateAuth0UserAsync(createUserDto);
            return Ok(result);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    private string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private bool HasPermission(string permission)
    {
        return User.HasClaim(c => c.Type == "permissions" && c.Value == permission);
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
