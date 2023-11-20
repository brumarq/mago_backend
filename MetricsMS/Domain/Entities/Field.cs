namespace Domain.Entities;

public class Field : BaseEntity
{
    public string? Name { get; set; }
    public int UnitId { get; set; }
    public int DeviceTypeId { get; set; }
    public bool Loggable { get; set; }
    public ICollection<LogValue>? LogValues { get; set; } = new List<LogValue>();
}