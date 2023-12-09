namespace Domain.Entities;

public class User : BaseEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PwHash { get; set; }
    public string? Salt { get; set; }
    public bool SysAdmin { get; set; }
    public string? Auth0Id { get; set; }
}