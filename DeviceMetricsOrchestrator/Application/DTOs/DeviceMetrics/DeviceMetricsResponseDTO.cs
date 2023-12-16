namespace Application.DTOs.DeviceMetrics
{
    public class DeviceMetricsResponseDTO : BaseDTO
    {
        public float? Value { get; set; }
        public FieldDTONew? Field { get; set; }
        public LogCollectionDTONew? LogCollection { get; set; }
    }
}
