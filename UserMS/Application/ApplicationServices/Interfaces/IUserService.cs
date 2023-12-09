using Application.DTOs;
using Domain.Entities;

namespace Application.ApplicationServices.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    Task<UserResponseDTO> GetUserByIdAsync(int id);
    Task<UserResponseDTO> UpdateUserAsync(int id, CreateUserDTO userDTO);
    Task<bool> DeleteUserAsync(int id);
}