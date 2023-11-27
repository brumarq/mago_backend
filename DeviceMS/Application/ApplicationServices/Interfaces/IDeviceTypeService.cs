using Application.DTOs;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceTypeService
    {
        Task<DeviceTypeResponseDTO> CreateDeviceTypeAsync(CreateDeviceTypeDTO deviceType);
        Task<IEnumerable<DeviceTypeResponseDTO>> GetDeviceTypesAsync();
        Task<DeviceTypeResponseDTO> GetDeviceTypeByIdAsync(int id);
        //TODO: Task<DeviceTypeResponseDTO> GetDeviceTypeByDeviceIdAsync(int deviceId);
        Task<bool?> UpdateDeviceTypeAsync(int id, UpdateDeviceTypeDTO deviceType);
    }
}