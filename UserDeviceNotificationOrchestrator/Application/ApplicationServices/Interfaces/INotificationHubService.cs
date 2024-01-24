using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices.Interfaces
{
    public interface INotificationHubService
    {
        Task SendNotificationToNotificationHub(NotificationResponseDTO notificationResponseDTO);
    }
}
