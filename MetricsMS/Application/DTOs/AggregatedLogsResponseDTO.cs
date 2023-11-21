namespace Application.DTOs
{
    public class AggregatedLogsResponseDTO : BaseDTO
    {
        public string? Type { get; set; }
        public DateOnly Date { get; set; }
        public double AverageValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
