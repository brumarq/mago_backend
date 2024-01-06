namespace Application.DTOs.Metrics
{
    public class MetricsResponseDTO : BaseDTO
    {
        /// <summary>
        /// Value (current) of LogValue
        /// </summary>
        public float? Value { get; set; }
        /// <summary>
        /// Field object
        /// </summary>
        public FieldDTO? Field { get; set; }
        /// <summary>
        /// LogCollection object
        /// </summary>
        public LogCollectionDTO? LogCollection { get; set; }
    }
}
