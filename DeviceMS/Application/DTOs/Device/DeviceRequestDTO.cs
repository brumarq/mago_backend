namespace Application.DTOs.Device;

public class DeviceRequestDTO
{
    public string Name { get; set; }
    public int DeviceTypeId { get; set; }
    public bool SendSettingsAtConn { get; set; }
    public bool SendSettingsNow { get; set; }
    public string AuthId { get; set; }
    public string Password { get; set; }
}