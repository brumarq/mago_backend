namespace Application.DTOs.Setting;

public class CreateSettingValueDTO
{
    public float Value { get; set; }
    public SettingDTO? Setting { get; set; }
    public const string UpdateStatus = "New";
    public int DeviceId { get; set; }
    public int UserId { get; set; }
}