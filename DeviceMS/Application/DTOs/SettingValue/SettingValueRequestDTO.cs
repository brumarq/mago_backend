using Application.DTOs.Setting;

namespace Application.DTOs.SettingValue;

public class SettingValueRequestDTO
{
    public float Value { get; set; }
    public SettingRequestDTO? Setting { get; set; }
    public int DeviceId { get; set; }
    public int UserId { get; set; }
}