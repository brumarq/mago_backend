namespace Domain.Entities;

public class Unit : BaseEntity
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public float? Factor { get; set; }
    public float? Offset { get; set; }
}