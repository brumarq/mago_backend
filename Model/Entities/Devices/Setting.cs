using Model.Enums;

namespace Model.Entities.Devices
{
    public class Setting : BaseEntity
    {
        public string? Name { get; set; }
        public double DefaultValue { get; set; }
        public Unit? Unit { get; set; }
        public DeviceType? DeviceType { get; set; }
        public Role ViewedBy { get; set; }
        public Role EditedBy { get; set; }
    }
}
