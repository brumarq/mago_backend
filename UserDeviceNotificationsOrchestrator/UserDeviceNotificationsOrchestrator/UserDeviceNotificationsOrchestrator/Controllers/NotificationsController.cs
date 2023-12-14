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

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<NotificationResponseDTO>> GetNotificationsForDeviceAsync(int deviceId)
        {
            try
            {
                var notificationDTO = await _notificationService.GetNotificationsByDeviceIdAsync(deviceId);
                if (notificationDTO == null)
                {
                    return NotFound();
                }

                return Ok(notificationDTO);
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
                var response = await _notificationService.CreateNotificationAsync(createNotificationDTO);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var notificationResponseDTO = JsonConvert.DeserializeObject<NotificationResponseDTO>(responseContent);
                    return Ok(notificationResponseDTO);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }



    }
}
