using Application.DTOs;
using Application.Enums;
using Domain.Entities;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0Service
{
    Task<UserDTO> CreateAuth0UserAsync(CreateUserDTO createUserDTO);
    Task<UserDTO> GetUser(string userId);

    Task<List<UserCompressedDTO>> GetAllUsers(int pageNumber = 1, int pageSize = 100, UserRole? role = null);
    Task<UserDTO> UpdateUserAsync(string userId, UpdateUserDTO updateUserDto);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> UpdateUserDetailsInAuth0(string userId, object userDetails);

}