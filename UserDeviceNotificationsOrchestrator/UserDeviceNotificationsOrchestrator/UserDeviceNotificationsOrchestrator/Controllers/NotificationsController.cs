using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetAllNotificationsAsync()
        //{
        //    try
        //    {
        //        var notifications = await _notificationService.GetAllNotificationsAsync();
        //        return Ok(notifications);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        // POST /<notifications>
        [HttpPost]
            public async Task<ActionResult<NotificationResponse>> CreateNotificationAsync([FromBody] CreateNotification createNotification)
            {
            return Ok();
            }

    }
}
