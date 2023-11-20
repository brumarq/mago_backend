namespace Domain.Entities;

public class DeviceLocation : BaseEntity
{
    public Device? Device { get; set; }
    public Location? Location { get; set; }
    public string? Ownership { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
}