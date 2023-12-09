namespace Application.DTOs;

public class UserResponseDTO : BaseDTO
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool SysAdmin { get; set; } // maybe this too because only an admin can set this
    public string? Auth0Id { get; set; }
}