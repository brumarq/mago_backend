using Application.DTOs.Misc;

namespace Application.DTOs.Setting;

public class SettingResponseDTO : BaseDTO
{
    public string? Name { get; set; }
    public float? DefaultValue { get; set; }
    public UnitDTO? Unit { get; set; }
}