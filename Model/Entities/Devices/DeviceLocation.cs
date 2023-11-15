using Model.Enums;

namespace Model.Entities.Devices
{
    public class DeviceLocation : BaseEntity
    {
        public Device? Device { get; set; }
        public Location? Location { get; set; }
        public Ownership Ownership { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime RemovedAt { get; set; }
    }
}
