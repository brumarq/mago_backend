using Application.DTOs;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0Service
{
    Task<Auth0UserResponse> CreateAuth0UserAsync(CreateUserDTO createUserDTO);

}