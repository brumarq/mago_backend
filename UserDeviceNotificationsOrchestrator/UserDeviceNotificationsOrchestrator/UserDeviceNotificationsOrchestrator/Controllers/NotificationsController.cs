using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserDeviceNotificationsOrchestrator.Controllers
{
    [Route("orchestrator/notification")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            this._notificationService = notificationService;
        }


        [HttpGet("users/{userId}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationForUserOnStatusTypeAsync(int userId)
        {
            try
            {
                var notificationResponseDTO = await _notificationService.GetNotificationsByDeviceIdAsync(userId);
                return Ok(notificationResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId)
        {
            try
            {
                var notificationDTOs = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId);
                return Ok(notificationDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST /<notifications>
        [HttpPost]
        public async Task<ActionResult<NotificationResponseDTO>> CreateNotificationAsync([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            try
            {
                var notificationResponseDTO = await _notificationService.CreateNotificationAsync(createNotificationDTO);
                return Ok(notificationResponseDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }



    }
}
