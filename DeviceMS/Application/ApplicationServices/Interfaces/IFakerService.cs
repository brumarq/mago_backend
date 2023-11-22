using Application.DTOs;
using Domain.Entities;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFakerService
    {
        #region Devices
        Task<IEnumerable<Device>> GetFakeDevicesAsync();
        Task CreateFakeDeviceAsync(Device device);
        #endregion

        #region DeviceTypes
        Task<IEnumerable<DeviceType>> GetFakeDeviceTypesAsync();
        Task CreateFakeDeviceTypeAsync(DeviceType deviceType);
        Task UpdateFakeDeviceTypeAsync(int id, DeviceType deviceType);
        #endregion

        #region DeviceSettings
        Task<IEnumerable<SettingValue>> GetFakeSettingValues();
        #endregion
    }
}
