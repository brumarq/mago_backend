using System.Security.Claims;
using System.Text;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

    // GET: /customers/7
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(string id)
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
            /*}*/

        return Ok(userDTO);
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

    // PUT: /users
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDto)
    {
        try
        {
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
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO createUserDTO)
    {
        try
        {
            var result = await _auth0Service.CreateAuth0UserAsync(createUserDTO);
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


}