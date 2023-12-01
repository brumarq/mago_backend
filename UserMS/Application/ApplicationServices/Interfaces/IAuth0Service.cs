using Application.DTOs;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0Service
{
    Task<CreateAuth0UserResponseDto> CreateAuth0UserAsync(CreateUserDTO createUserDTO);

}