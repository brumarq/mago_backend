using Application.DTOs;
using Application.DTOs.DeviceType;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceTypeService
    {
        Task<DeviceTypeResponseDTO> CreateDeviceTypeAsync(CreateDeviceTypeDTO deviceType);
        Task<IEnumerable<DeviceTypeResponseDTO>> GetDeviceTypesAsync(int? pageNumber = null, int? pageSize = null);

        Task<DeviceTypeResponseDTO> GetDeviceTypeByIdAsync(int id);
        Task<IEnumerable<LabelValueOptionDTO>> GetDeviceTypeDropdown(int? pageNumber = null, int? pageSize = null);

        //TODO: Task<DeviceTypeResponseDTO> GetDeviceTypeByDeviceIdAsync(int deviceId);
        Task<bool?> UpdateDeviceTypeAsync(int id, UpdateDeviceTypeDTO deviceType);
    }
}