using Domain.Entities;

namespace Application.DTOs
{
    public class DeviceMetricsResponseDTO : BaseDTO
    {
        public int? DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public ICollection<LogValue>? Values { get; set; }
        public LogCollectionType? LogCollectionType { get; set; }
    }
}
