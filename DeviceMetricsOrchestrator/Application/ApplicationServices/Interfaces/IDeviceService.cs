using Application.DTOs.Device;

namespace Application.ApplicationServices.Interfaces;

public interface IDeviceService
{
    Task<bool> DeviceExists(int deviceId);
}