namespace Application.DTOs.Setting;

public class SettingRequestDTO
{
    public string? Name { get; set; }
    public float? DefaultValue { get; set; }
    public int UnitId { get; set; }
}