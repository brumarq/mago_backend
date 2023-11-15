using Microsoft.AspNetCore.Mvc;
using Model.DTOs.Users;
using Service.Interfaces;

namespace WebAPP.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserById(int id)
        {
            try
            {
                var userDTO = await _userService.GetUserByIdAsync(id);
                if (userDTO == null)
                {
                    return NotFound();
                }
                return Ok(userDTO);
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
                var customers = await _userService.GetAllUsersAsync();
                return Ok(customers);
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
                bool? result = await _userService.UpdateUserAsync(id, createUserDTO);

                if (result == null) return NotFound();
                return (bool)result ? NoContent() : StatusCode(500, "Internal server error: updating record failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: /users
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                var result = await _userService.CreateUserAsync(createUserDTO);
                if (result == null)
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
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

