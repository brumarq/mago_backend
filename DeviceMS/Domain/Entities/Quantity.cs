namespace Domain.Entities;

public class Quantity : BaseEntity
{
    public string? Name { get; set; }
    public ICollection<Unit>? Unit { get; set; } = new List<Unit>();
    public int TestInt { get; set; }
}