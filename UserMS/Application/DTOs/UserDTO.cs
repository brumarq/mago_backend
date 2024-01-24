using Domain.Entities;

namespace Application.DTOs;

public class UserDTO
{
    public User? User { get; set; }
    public string Role { get; set; }
}