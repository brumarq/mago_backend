using Application.DTOs;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0Service
{
    Task<Auth0UserResponseDto> CreateAuth0UserAsync(CreateUserDTO createUserDTO);
    Task<Auth0UserResponseDto> GetUser(string userId);

    
}