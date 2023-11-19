namespace DeviceService.Core.Domain.Models;

public class Unit : BaseEntity
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public float? Factor { get; set; }
    public float? Offset { get; set; }
    
    public Quantity? Quantity { get; set; }
    public ICollection<Quantity>? BaseOfQuantities { get; set; } = new List<Quantity>();
}