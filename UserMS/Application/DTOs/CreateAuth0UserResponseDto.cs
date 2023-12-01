namespace Application.DTOs;

public class CreateAuth0UserResponseDto
{
    public Auth0UserResponse User { get; set; }
    public string Role { get; set; }
}