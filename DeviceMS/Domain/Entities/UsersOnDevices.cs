namespace Domain.Entities;

public class UsersOnDevices : BaseEntity
{
    public string? UserId { get; set; }
    public int DeviceId { get; set; } // for mapping
    public Device? Device { get; set; }
}