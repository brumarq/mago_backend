using Application.DTOs;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceSettingsService
    {
        Task<IEnumerable<DeviceSettingsResponseDTO>> GetSettingsForDeviceAsync(int deviceId);
    }
}
