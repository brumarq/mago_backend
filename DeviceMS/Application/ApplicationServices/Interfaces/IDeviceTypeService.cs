using Application.DTOs;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceTypeService
    {
        Task<IEnumerable<DeviceTypeResponseDTO>> GetDeviceTypesAsync();
        Task<CreateDeviceTypeDTO> CreateDeviceTypeAsync(CreateDeviceTypeDTO deviceType);
        Task<UpdateDeviceTypeDTO> UpdateDeviceTypeAsync(int id, UpdateDeviceTypeDTO deviceType);
    }
}
