using Application.DTOs;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceService
    {
        Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId);
    }
}
