using Application.DTOs;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0Service
{
    Task<Auth0UserResponseDto> CreateAuth0UserAsync(CreateUserDTO createUserDTO);
    Task<Auth0UserResponseDto> GetUser(string userId);

    Task<List<Auth0UserResponseDto>> GetAllUsers();
    Task<Auth0UserResponseDto> UpdateUserAsync(string userId, UpdateUserDTO updateUserDto);
    Task<bool> DeleteUserAsync(string userId);

}