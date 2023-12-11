namespace Application.DTOs.Metrics
{
    public class MetricsResponseDTO : BaseDTO
    {
        public float? Value { get; set; }
        public FieldDTO? Field { get; set; }
        public LogCollectionDTO? LogCollection { get; set; }
    }
}
