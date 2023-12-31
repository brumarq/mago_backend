using Application.DTOs.UsersOnDevices;

namespace Application.ApplicationServices.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> IsDeviceAccessibleToUser(string loggedInUserId, int deviceId);
        bool IsCorrectUserOrAdmin(string loggedInUserId, string userId);
    }
}
