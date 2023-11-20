namespace DeviceService.Core.Domain.Models;

public class UsersOnDevices : BaseEntity
{
    public int UserId { get; set; }
    public Device? Device { get; set; }
    public string? Role { get; set; }
    public bool ConnectionMail { get; set; }
}