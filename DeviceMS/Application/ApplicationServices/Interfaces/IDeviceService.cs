using Application.DTOs;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceService
    {
        Task<IEnumerable<DeviceResponseDTO>> GetAllDevicesAsync();
        Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId);
        Task<CreateDeviceDTO> CreateDeviceAsync(CreateDeviceDTO createDeviceDTO);
    }
}
