namespace Application.DTOs.Misc;

public class QuantityDTO : BaseDTO
{
    public string? Name { get; set; }
    public UnitDTO? BaseUnit { get; set; }
}