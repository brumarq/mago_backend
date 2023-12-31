namespace Application.DTOs.Misc;

public class QuantityDTO : BaseDTO
{
    public string? Name { get; set; }
    public UnitResponseDTO? BaseUnit { get; set; }
}