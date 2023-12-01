namespace Application.DTOs;

public class CreateUserDTO // email/username??
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool SysAdmin { get; set; }
    public string? Password { get; set; }
}