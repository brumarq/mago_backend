namespace Application.DTOs.UsersOnDevices
{
    public class UsersOnDevicesResponseDTO
    {
        public string? UserId { get; set; }
        public int DeviceId { get; set; }
        public string? Role { get; set; }
        public bool ConnectionMail { get; set; }
    }
}
