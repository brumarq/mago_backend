using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Devices
{
    public class Device : BaseEntity
    {
        public string? Name { get; set; }
        public bool SendSettingsAtConn { get; set; }
        public bool SendSettingsNow { get; set; }
        public string? AuthId { get; set; }
        public string? PwHash { get; set; }
        public string? Salt { get; set; }

        [ForeignKey("DeviceTypeId")] // Just for readability (could also change the name if you want)
        public DeviceType? DeviceType { get; set; }
    }
}
