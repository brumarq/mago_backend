namespace Application.DTOs;

public class CreateUserDTO // email/username??
{
    public string? FamilyName { get; set; }
    public string? GivenName { get; set; }
    public string? Email { get; set; }
    public bool SysAdmin { get; set; }
    public string? Password { get; set; }
}