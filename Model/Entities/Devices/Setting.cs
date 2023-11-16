using System.ComponentModel.DataAnnotations.Schema;
using Model.Enums;

namespace Model.Entities.Devices
{
    public class Setting : BaseEntity
    {
        public string? Name { get; set; }
        public float? DefaultValue { get; set; }
        public Unit? Unit { get; set; }
        public DeviceType? DeviceType { get; set; }
        public string? ViewedBy { get; set; }
        public string? EditedBy { get; set; }
        public ICollection<SettingValue>? Values { get; set; } = new HashSet<SettingValue>();
    }
}
