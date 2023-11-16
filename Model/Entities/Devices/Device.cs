using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Model.Entities.Devices
{
    public class Device : BaseEntity
    {
        public string? Name { get; set; }
        public int DeviceTypeId { get; set; }
        public DeviceType? Type { get; set; }
        public bool SendSettingsAtConn { get; set; }
        public bool SendSettingsNow { get; set; }
        public string? AuthId { get; set; }
        public string? PwHash { get; set; }
        public string? Salt { get; set; }
        public ICollection<SettingValue>? SettingValues { get; set; } = new HashSet<SettingValue>();
        public ICollection<UsersOnDevices>? Users { get; set; } = new HashSet<UsersOnDevices>();
        public ICollection<LogCollection>? LogCollections { get; set; } = new HashSet<LogCollection>();
        public ICollection<Status>? Statusses { get; set; } = new HashSet<Status>();
        public ICollection<FileSend>? FileSends { get; set; } = new HashSet<FileSend>();
        public ICollection<DeviceLocation>? DeviceLocations { get; set; } = new HashSet<DeviceLocation>();
        public ICollection<UserOnStatusType>? UserOnStatusTypes { get; set; } = new HashSet<UserOnStatusType>();
    }
}
