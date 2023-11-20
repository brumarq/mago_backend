namespace Domain.Entities;

public class Status : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public StatusType? StatusType { get; set; }
    public string? Message { get; set; }
    public int DeviceId { get; set; }
}