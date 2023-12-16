namespace Application.DTOs.Metrics
{
    public class AggregatedLogsResponseDTO : BaseDTO
    {
        public float AverageValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public int DeviceId { get; set; }
        public FieldDTO? Field { get; set; }
    }
}
