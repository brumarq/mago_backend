namespace Application.DTOs.Device
{
    public class DeviceResponseDTO : BaseDTO
    {
        /// <summary>
        /// Name of the device
        /// </summary>
        /// <example>Device 1</example>
        public string? Name { get; set; }
        /// <summary>
        /// Device Type object
        /// </summary>
        public DeviceTypeResponseDTO? DeviceType { get; set; }
        /// <summary>
        /// Setting to send settings during connection
        /// </summary>
        public bool SendSettingsAtConn { get; set; }
        /// <summary>
        /// Settings to send settings now
        /// </summary>
        public bool SendSettingsNow { get; set; }
        /// <summary>
        /// Identifier for the device
        /// </summary>
        public string? AuthId { get; set; }
    }
}
