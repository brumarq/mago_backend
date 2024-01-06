using Application.DTOs.Device;

namespace Application.ApplicationServices.Interfaces;

public interface IDeviceService
{
    Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId);
    Task CheckDeviceExistence(int deviceId);
}