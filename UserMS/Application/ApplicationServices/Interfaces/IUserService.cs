using Application.DTOs;

namespace Application.ApplicationServices.Interfaces;

public interface IUserService
{
    Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createUserDTO);
    Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    Task<UserResponseDTO> GetUserByIdAsync(int id);
    Task<UserResponseDTO> UpdateUserAsync(int id, CreateUserDTO userDTO);
    Task<bool> DeleteUserAsync(int id);
}