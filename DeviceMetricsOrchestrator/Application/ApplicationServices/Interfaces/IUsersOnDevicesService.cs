using Application.DTOs.UsersOnDevices;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUsersOnDevicesService
    {
        Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId);
    }
}
