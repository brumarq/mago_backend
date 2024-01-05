using Application.DTOs.Device;

namespace Application.DTOs.UsersOnDevices
{
    public class UsersOnDevicesResponseDTO : BaseDTO
    {
        /// <summary>
        /// User unique identifier
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// Device object
        /// </summary>
        public DeviceResponseDTO? Device { get; set; }
    }
}