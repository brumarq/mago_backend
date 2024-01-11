using TestDomain.Metrics.Device;
using TestDomain.Metrics.Field;

namespace TestDomain.Metrics
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
        /// <summary>
        /// Reference date (start date of when the aggregated logs were calculated)
        /// </summary>
        public string? ReferenceDate { get; set; }
    }

}
