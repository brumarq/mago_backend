namespace Application.DTOs.Metrics
{
    public class AggregatedLogsResponseDTO : BaseDTO
    {
        /// <summary>
        /// Average value of LogValue
        /// </summary>
        public float AverageValue { get; set; }
        /// <summary>
        /// Minimum value of LogValue
        /// </summary>
        public float MinValue { get; set; }
        /// <summary>
        /// Maximum value of LogValue
        /// </summary>
        public float MaxValue { get; set; }
        /// <summary>
        /// Device unique identifier
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// Field object
        /// </summary>
        public FieldDTO? Field { get; set; }
        /// <summary>
        /// Reference date (start date of when the aggregated logs were calculated)
        /// </summary>
        public string? ReferenceDate { get; set; }
    }
}
