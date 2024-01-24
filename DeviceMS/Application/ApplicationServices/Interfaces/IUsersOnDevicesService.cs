using Application.DTOs.UsersOnDevices;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUsersOnDevicesService
    {
        Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId, int? pageNumber = null, int? pageSize = null);
        Task<CreateUserOnDeviceDTO> CreateUserOnDeviceAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO);
        Task<bool> DeleteUserOnDeviceAsync(int id);
    }
}
