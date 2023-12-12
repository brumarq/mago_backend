namespace Application.ApplicationServices.Interfaces;

public interface IDeviceService
{
    Task EnsureDeviceExists(int deviceId);
}