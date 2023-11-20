namespace Domain.Entities;

public class LogCollection : BaseEntity
{
    public int? DeviceId { get; set; }
    public DateTime Timestamp { get; set; }
    public ICollection<LogValue>? Values { get; set; } = new List<LogValue>();
    public LogCollectionType? LogCollectionType { get; set; }
}