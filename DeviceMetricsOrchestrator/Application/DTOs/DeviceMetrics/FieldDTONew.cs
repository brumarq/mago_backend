using Application.DTOs.Device;
using Application.DTOs.Misc;

namespace Application.DTOs.DeviceMetrics
{
    public class FieldDTONew : BaseDTO
    {
        public string? Name { get; set; }
        public UnitDTO? Unit { get; set; }
        public DeviceTypeResponseDTO? DeviceType { get; set; }
        public bool Loggable { get; set; }
    }
}
