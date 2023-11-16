namespace Model.Entities.Devices
{
    public class DeviceType : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Device>? Devices { get; set; } = new HashSet<Device>();
        public ICollection<Setting>? Settings { get; set; } = new HashSet<Setting>();
        public ICollection<Field>? Fields { get; set; } = new HashSet<Field>();
    }
}
