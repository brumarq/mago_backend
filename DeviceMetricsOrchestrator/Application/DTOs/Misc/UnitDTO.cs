namespace Application.DTOs.Misc;

public class UnitDTO : BaseDTO
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public float? Factor { get; set; }
    public float? Offset { get; set; }
    //public QuantityDTO? Quantity { get; set; } gone for now
}