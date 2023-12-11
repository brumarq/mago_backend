public interface IDeviceTypeService
{
    Task<bool> DeviceTypeExists(int deviceTypeId);
}