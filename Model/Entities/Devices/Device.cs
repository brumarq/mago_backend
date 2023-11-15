namespace Model.Entities.Devices
{
    public class Device : BaseEntity
    {
        public string? Name { get; set; }
        public DeviceType? DeviceType { get; set; }
        public bool SendSettingsAtCoon { get; set; }
        public bool SendSettingsNow { get; set; }
        public string? AuthId { get; set; }
        public string? Password { get; set; }
    }
}
