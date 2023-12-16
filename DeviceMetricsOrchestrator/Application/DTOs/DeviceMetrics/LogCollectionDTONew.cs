using Application.DTOs.Device;
using Application.DTOs.Metrics;

namespace Application.DTOs.DeviceMetrics
{
    public class LogCollectionDTONew : BaseDTO
    {
        public DeviceResponseDTO? Device { get; set; }
        public LogCollectionTypeDTO? LogCollectionType { get; set; }
    }
}
