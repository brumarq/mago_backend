using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("orchestrator/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        /// Delete User. Accessible by Admin.
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns>Returns OK</returns>
        /// <response code="200">Returns OK.</response>
        /// <response code="404">Device not found.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{userID}")]
        [Authorize("Admin")]
        public async Task<ActionResult> DeleteUser(string userID)
        {
            try
            {
                await _userService.DeleteUser(userID);
                return Ok();
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
    }
}
