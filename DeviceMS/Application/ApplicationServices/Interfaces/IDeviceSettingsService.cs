using Application.DTOs.Setting;
using Application.DTOs.SettingValue;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceSettingsService
    {
        Task<SettingValueResponseDTO> AddSettingToDevice(CreateSettingValueDTO newSettingValue);
        Task<IEnumerable<SettingValueResponseDTO>> GetSettingsForDeviceAsync(int deviceId, int? pageNumber = null, int? pageSize = null);
        // Task<bool?> UpdateSettingAsync(int id, UpdateSettingValueDTO updateSettingValueDto); 
        // Task<bool> DeleteSettingFromDeviceAsync(int id);
    }
}