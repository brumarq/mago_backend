using Application.DTOs;
using Domain.Entities;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFakerService
    {
        Task<IEnumerable<Device>> GetFakeDevicesAsync();
        Task CreateFakeDeviceAsync(Device device);
        Task<IEnumerable<DeviceType>> GetFakeDeviceTypesAsync();
        Task CreateFakeDeviceTypeAsync(DeviceType deviceType);
        Task UpdateFakeDeviceTypeAsync(int id, DeviceType deviceType);
    }
}
