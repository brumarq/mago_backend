using UserService.Application.DTOs;

namespace UserService.Application.Services.Interfaces;

public interface IUserService
{
    Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createUserDTO);
    Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    Task<UserResponseDTO> GetUserByIdAsync(int id);
    Task<bool?> UpdateUserAsync(int id, CreateUserDTO userDTO);
    Task<bool> DeleteUserAsync(int id);
}