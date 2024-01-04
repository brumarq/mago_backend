namespace Application.DTOs.Misc;

public class UnitResponseDTO : BaseDTO
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public float? Factor { get; set; }
    public float? Offset { get; set; }
}