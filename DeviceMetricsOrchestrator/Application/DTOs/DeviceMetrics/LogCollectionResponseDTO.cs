using Application.DTOs.Device;
using Application.DTOs.Metrics;

namespace Application.DTOs.DeviceMetrics
{
    public class LogCollectionResponseDTO : BaseDTO
    {
        /// <summary>
        /// Device object
        /// </summary>
        public DeviceResponseDTO? Device { get; set; }
        /// <summary>
        /// LogCollectionType object
        /// </summary>
        public LogCollectionTypeDTO? LogCollectionType { get; set; }
    }
}
