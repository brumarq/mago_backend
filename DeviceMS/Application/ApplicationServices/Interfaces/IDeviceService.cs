using Application.DTOs.Device;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceService
    {
        Task<DeviceResponseDTO> CreateDeviceAsync(CreateDeviceDTO newDeviceDto);
        Task<IEnumerable<DeviceResponseDTO>> GetDevicesAsync(int? pageNumber = null, int? pageSize = null);
        Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId);
        Task<bool?> UpdateDeviceAsync(int id, UpdateDeviceDTO updateDeviceDto);
    }
}