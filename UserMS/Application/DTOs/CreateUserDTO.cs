namespace Application.DTOs;

public class CreateUserDTO // email/username??
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool SysAdmin { get; set; } // maybe this too because only an admin can set this
    public string? Password { get; set; }
}