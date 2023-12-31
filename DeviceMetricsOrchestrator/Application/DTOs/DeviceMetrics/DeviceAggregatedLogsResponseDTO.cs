using Application.DTOs.Device;

namespace Application.DTOs.DeviceMetrics
{
    public class DeviceAggregatedLogsResponseDTO : BaseDTO
    {
        public float AverageValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public DeviceResponseDTO? Device { get; set; }
        public FieldResponseDTO? Field { get; set; }
    }
}
