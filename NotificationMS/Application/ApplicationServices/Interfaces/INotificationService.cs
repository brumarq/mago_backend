using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO);
        //Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync();
        Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsPagedAsync(int pageNumber, int pageSize);
        Task<NotificationResponseDTO> GetNotificationByIdAsync(int id);
        Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceIdAsync(int deviceId);
        Task<StatusTypeDTO> GetStatusTypeByIdAsync(int id);
        Task<StatusTypeDTO> CreateStatusTypeAsync(CreateStatusTypeDTO statusTypeDTO);
        Task DeleteStatusTypeAsync(int id);
        Task<StatusTypeDTO> UpdateStatusTypeAsync(int id, CreateStatusTypeDTO statusTypeDTO);
    }
}
