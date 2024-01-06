using Application.DTOs;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0Service
{
    Task<UserDTO> CreateAuth0UserAsync(CreateUserDTO createUserDTO);
    Task<UserDTO> GetUser(string userId);

    Task<List<UserCompressedDTO>> GetAllUsers();
    Task<UserDTO> UpdateUserAsync(string userId, UpdateUserDTO updateUserDto);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> UpdateUserDetailsInAuth0(string userId, object userDetails);

}