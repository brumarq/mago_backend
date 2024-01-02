namespace Domain.Entities;

public class Status : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public int StatusTypeId { get; set; }
    public string? Message { get; set; }
    public int DeviceId { get; set; }
}