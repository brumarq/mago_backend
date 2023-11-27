using Application.DTOs.Setting;

namespace Application.DTOs.SettingValue;

public class SettingValueResponseDTO : BaseDTO
{
    public float Value { get; set; }
    public SettingResponseDTO? Setting { get; set; }
    public string? UpdateStatus { get; set; }
    public int DeviceId { get; set; }
    public int? UserId { get; set; }
}