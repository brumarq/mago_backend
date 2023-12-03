using System.Security.Claims;
using System.Text;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers;

[Route("users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IAuth0Service _auth0Service;

    public UserController(IUserService userService, IConfiguration configuration, IAuth0Service auth0Service)
    {
        _userService = userService;
        _configuration = configuration;
        _auth0Service = auth0Service;
    }

    // GET: /customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Auth0UserResponse>> GetUserById(string id)
    {
        try
        {
            // Extract the user's ID and roles/permissions from the JWT token
            /*var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hasAdminPermission = User.HasClaim(c => c.Type == "permissions" && c.Value == "manage:users\t");
            var hasClientPermission = User.HasClaim(c => c.Type == "permissions" && c.Value == "manage:own-users\t");

            // Check if the user has the right permissions
            if (hasAdminPermission || (hasClientPermission && userIdClaim == id.ToString()))
            {*/
                var userDTO = await _auth0Service.GetUser(id);
                if (userDTO == null)
                {
                    return NotFound();
                }

                return Ok(userDTO);
            /*}*/ 
            
            return Forbid(); // or return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET: /customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _auth0Service.GetAllUsersWithRolesAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // PUT: /users
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] CreateUserDTO createUserDTO)
    {
        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, createUserDTO);

            if (updatedUser == null)
            {
                return NotFound("User not found or update failed.");
            }

            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    // DELETE: /users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound($"Customer with id {id} not found.");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    
    [HttpPost]
    public async Task<ActionResult<Auth0UserResponse>> CreateUser([FromBody] CreateUserDTO createUserDTO)
    {
        try
        {
            var hasAdminPermission = User.HasClaim(c => c.Type == "permissions" && c.Value == "manage:users\t");

            if (!hasAdminPermission)
            {
                return Unauthorized();
            }

            
            var result = await _auth0Service.CreateAuth0UserAsync(createUserDTO);
            
            if (result.User == null || result.Role == null)
            {
                return StatusCode(500, "The customer could not be created.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}