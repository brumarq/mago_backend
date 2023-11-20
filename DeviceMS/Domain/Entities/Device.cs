namespace Domain.Entities;

public class Device : BaseEntity
{
    public string? Name { get; set; }
    public DeviceType? DeviceType { get; set; }
    public bool SendSettingsAtConn { get; set; }
    public bool SendSettingsNow { get; set; }
    public string? AuthId { get; set; }
    public string? PwHash { get; set; }
    public string? Salt { get; set; }
}