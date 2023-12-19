using Application.DTOs.Device;

namespace Application.DTOs.DeviceMetrics
{
    public class FieldDTONew : BaseDTO
    {
        public string? Name { get; set; }
        public int UnitId { get; set; }
        public DeviceTypeResponseDTO? DeviceType { get; set; }
        public bool Loggable { get; set; }
    }
}
