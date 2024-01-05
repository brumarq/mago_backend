namespace Application.DTOs.Misc;

public class UnitResponseDTO : BaseDTO
{
    /// <summary>
    /// Name of the unit
    /// </summary>
    /// <example>Kelvin</example>
    public string? Name { get; set; }
    /// <summary>
    /// Symbol of the unit
    /// </summary>
    /// <example>K</example>
    public string? Symbol { get; set; }
    /// <summary>
    /// Factor by which the unit defers from the base on a x10 scale (x10^3)
    /// </summary>
    public float? Factor { get; set; }
    /// <summary>
    /// Offset by how much the unit defers from base on a normal scale (+27)
    /// </summary>
    public float? Offset { get; set; }
}