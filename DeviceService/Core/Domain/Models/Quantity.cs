namespace DeviceService.Core.Domain.Models;

public class Quantity : BaseEntity
{
    public string? Name { get; set; }
    public Unit? BaseUnit { get; set; }
    public ICollection<Unit>? Units { get; set; } = new List<Unit>();
}