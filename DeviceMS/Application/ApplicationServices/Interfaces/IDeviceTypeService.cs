using Application.DTOs;
using Application.DTOs.DeviceType;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceTypeService
    {
        Task<DeviceTypeResponseDTO> CreateDeviceTypeAsync(CreateDeviceTypeDTO deviceType);
        Task<IEnumerable<DeviceTypeResponseDTO>> GetDeviceTypesAsync();

        Task<DeviceTypeResponseDTO> GetDeviceTypeByIdAsync(int id);
        Task<IEnumerable<LabelValueOptionDTO>> GetDeviceTypeDropdown();

        //TODO: Task<DeviceTypeResponseDTO> GetDeviceTypeByDeviceIdAsync(int deviceId);
        Task<bool?> UpdateDeviceTypeAsync(int id, UpdateDeviceTypeDTO deviceType);
    }
}