using Application.DTOs;
using Application.DTOs.Setting;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceSettingsService
    {
        Task<IEnumerable<SettingValueResponseDTO>> GetSettingsForDeviceAsync(int deviceId);
        Task<SettingValueResponseDTO> AddSettingsToDevice(CreateSettingValueDTO newSettingValue);
    }
}
