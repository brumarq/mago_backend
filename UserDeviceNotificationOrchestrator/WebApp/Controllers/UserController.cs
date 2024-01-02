﻿using Application.ApplicationServices.Interfaces;
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
        
        [HttpDelete("{userID}")]
        [Authorize("Admin")]
        public async Task<ActionResult<UserOnDeviceResponseDTO>> DeleteUser(string userID)
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
