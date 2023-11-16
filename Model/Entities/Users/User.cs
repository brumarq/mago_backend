using Model.Entities.Devices;

namespace Model.Entities.Users
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PwHash { get; set; }
        public string? Salt { get; set; }
        public bool SysAdmin { get; set; }
        public ICollection<UsersOnDevices>? Devices { get; set; } = new List<UsersOnDevices>();
        public ICollection<SettingValue>? SettingValues { get; set; } = new List<SettingValue>();
        public ICollection<FileSend>? FileSends { get; set; } = new List<FileSend>();
        public ICollection<UserOnStatusType>? UserOnStatusTypes { get; set; } = new List<UserOnStatusType>();
    }
}
