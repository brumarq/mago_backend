namespace Domain.Entities;

public class SettingValue : BaseEntity
{
    public float Value { get; set; }
    public Setting? Setting { get; set; }
    public string? UpdateStatus { get; set; }
    public int DeviceId { get; set; }
    public Device? Device { get; set; }
    public string? UserId { get; set; }
}