using System.Security.Claims;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers;

[Route("users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuth0Service _auth0Service;

    public UserController(IConfiguration configuration, IAuth0Service auth0Service)
    {
        _configuration = configuration;
        _auth0Service = auth0Service;
    }

    // GET: /customers/5
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
        catch (Auth0Service.UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    // GET: /customers
    [HttpGet]
    [Authorize("Admin")]
    public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _auth0Service.GetAllUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // PUT: /userss
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
        catch (Auth0Service.UserUpdateException ex)
        {
            return BadRequest(ex.Message); // Or another appropriate status code
        }
        catch (Auth0Service.UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }    
    // DELETE: /users/{id}
    [HttpDelete("{id}")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            await _auth0Service.DeleteUserAsync(id);
            return Ok();
        }
        catch (Auth0Service.UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    
    [HttpPost]
    [Authorize("Admin")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO createUserDto)
    {
        try
        {
            var result = await _auth0Service.CreateAuth0UserAsync(createUserDto);
            return Ok(result);
        }
        catch (Auth0Service.UserCreationException ex)
        {
            return BadRequest(ex.Message); // Or another appropriate status code
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
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

}
