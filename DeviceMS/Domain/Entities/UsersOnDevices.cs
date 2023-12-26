namespace Domain.Entities;

public class UsersOnDevices : BaseEntity
{
    public string? UserId { get; set; }
    public Device? Device { get; set; }
    public string? Role { get; set; }
    public bool ConnectionMail { get; set; }
}