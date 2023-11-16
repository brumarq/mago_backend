using System.ComponentModel.DataAnnotations.Schema;
using Model.Enums;

namespace Model.Entities.Devices
{
    public class DeviceLocation : BaseEntity
    {
        public Ownership Ownership { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime RemovedAt { get; set; }

        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }

        [ForeignKey("LocationId")]
        public Location? Location { get; set; }
    }
}
