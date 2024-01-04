namespace Application.DTOs.UsersOnDevices
{
    public class UserOnDeviceResponseDTO : BaseDTO
    {
        public string? UserId { get; set; }
        public DeviceResponseDTO? Device { get; set; }
    }
}
