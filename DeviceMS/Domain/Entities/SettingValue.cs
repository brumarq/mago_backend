namespace Domain.Entities;

public class SettingValue : BaseEntity
{
    public float Value { get; set; }
    public Setting? Setting { get; set; }
    public string? UpdateStatus { get; set; }
    public Device? Device { get; set; }
    public int? UserId { get; set; }
}