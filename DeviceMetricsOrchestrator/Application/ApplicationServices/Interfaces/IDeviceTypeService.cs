using Application.DTOs.Device;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceTypeService
    {
        Task<DeviceTypeResponseDTO> GetDeviceTypeByIdAsync(int deviceTypeId);
        Task CheckDeviceTypeExistence(int deviceTypeId);
    }
}
