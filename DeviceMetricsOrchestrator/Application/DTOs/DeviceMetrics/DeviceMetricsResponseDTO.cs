namespace Application.DTOs.DeviceMetrics
{
    public class DeviceMetricsResponseDTO : BaseDTO
    {
        public float? Value { get; set; }
        public FieldResponseDTO? Field { get; set; }
        public LogCollectionResponseDTO? LogCollection { get; set; }
    }
}
