using Application.DTOs.Device;

namespace Application.DTOs.UsersOnDevices
{
    public class UsersOnDevicesResponseDTO : BaseDTO
    {
        public string? UserId { get; set; }
        public DeviceResponseDTO? Device { get; set; }
    }
}