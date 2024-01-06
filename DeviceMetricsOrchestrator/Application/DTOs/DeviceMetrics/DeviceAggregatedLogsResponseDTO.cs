using Application.DTOs.Device;

namespace Application.DTOs.DeviceMetrics
{
    public class DeviceAggregatedLogsResponseDTO : BaseDTO
    {
        /// <summary>
        /// Average value of a LogValue
        /// </summary>
        public float AverageValue { get; set; }
        /// <summary>
        /// Minimum value of a LogValue
        /// </summary>
        public float MinValue { get; set; }
        /// <summary>
        /// Maximum value of a LogValue
        /// </summary>
        public float MaxValue { get; set; }
        /// <summary>
        /// Device object
        /// </summary>
        public DeviceResponseDTO? Device { get; set; }
        /// <summary>
        /// Field object
        /// </summary>
        public FieldResponseDTO? Field { get; set; }
    }
}
