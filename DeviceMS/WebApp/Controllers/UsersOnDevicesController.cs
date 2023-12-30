﻿using Application.ApplicationServices.Interfaces;
using Application.DTOs.UsersOnDevices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class UsersOnDevicesController : ControllerBase
    {
        private readonly IUsersOnDevicesService _usersOnDevicesService;

        public UsersOnDevicesController(IUsersOnDevicesService usersOnDevicesService)
        {
            _usersOnDevicesService = usersOnDevicesService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<UsersOnDevicesResponseDTO>>> GetUsersOnDevicesByUserId(string userId)
        {
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
        public async Task<ActionResult<UsersOnDevicesResponseDTO>> CreateUsersOnDevicesEntry([FromBody] CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            try
            {
                var newDeviceType = await _usersOnDevicesService.CreateUserOnDeviceAsync(createUserOnDeviceDTO);

                return (newDeviceType == null)
                    ? StatusCode(500, "The UserOnDevicy entry could not be created.")
                    : Ok(newDeviceType);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int userOnDeviceId)
        {
            try
            {
                var deleteResult = await _usersOnDevicesService.DeleteUserOnDeviceAsync(userOnDeviceId);
                if (deleteResult)
                {
                    return NoContent(); 
                }
                else
                {
                    return NotFound($"UserOnDevice with ID {userOnDeviceId} was not found."); 
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
