using Application.DTOs.UsersOnDevices;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUsersOnDevicesService
    {
        Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId);
        Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByDeviceIdAsync(int deviceId);
        Task<CreateUserOnDeviceDTO> CreateUserOnDeviceAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO);
        Task<bool> DeleteUserOnDeviceAsync(int id);
    }
}
