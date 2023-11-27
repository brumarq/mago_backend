using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Device : BaseEntity
{
    public string? Name { get; set; }
    public int DeviceTypeId { get; set; }
    public DeviceType? DeviceType { get; set; }
    public bool SendSettingsAtConn { get; set; }
    public bool SendSettingsNow { get; set; }
    public string? AuthId { get; set; }
    public string? PwHash { get; set; }
    public string? Salt { get; set; }

    public string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt();
    }

    public string GenerateHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, Salt);
    }
}