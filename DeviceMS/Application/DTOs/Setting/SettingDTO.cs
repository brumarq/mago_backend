namespace Application.DTOs.Setting;

public class SettingDTO : BaseDTO
{
    public string? Name { get; set; }
    public float? DefaultValue { get; set; }
    public int UnitId { get; set; }
}