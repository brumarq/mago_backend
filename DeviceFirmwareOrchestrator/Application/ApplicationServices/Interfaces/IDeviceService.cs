namespace Application.ApplicationServices.Interfaces;

public interface IDeviceService
{
    Task<bool> DeviceExists(int deviceId);
}