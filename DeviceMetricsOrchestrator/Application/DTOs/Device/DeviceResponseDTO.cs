namespace Application.DTOs.Device
{
    public class DeviceResponseDTO : BaseDTO
    {
        public string? Name { get; set; }
        public DeviceTypeResponseDTO? DeviceType { get; set; }
        public bool SendSettingsAtConn { get; set; }
        public bool SendSettingsNow { get; set; }
        public string? AuthId { get; set; }
    }
}
