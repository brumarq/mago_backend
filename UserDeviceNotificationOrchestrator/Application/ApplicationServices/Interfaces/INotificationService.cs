using Application.DTOs;

namespace Application.ApplicationServices.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO);
        Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceIdAsync(int deviceId);
    }
}
