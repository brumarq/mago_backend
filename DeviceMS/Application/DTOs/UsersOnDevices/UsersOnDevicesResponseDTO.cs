using Application.DTOs.Device;

namespace Application.DTOs.UsersOnDevices
{
    public class UsersOnDevicesResponseDTO : BaseDTO
    {
        public string? UserId { get; set; }
        public int DeviceId { get; set; }
        public string? Role { get; set; }
        public bool ConnectionMail { get; set; }
    }
}
