using AutoMapper;
using UserService.Application.DTOs;
using UserService.Core.Domain.Models;

namespace UserService.Application.Profiles;

public class Profiles : Profile
{
    public Profiles()
    {
        #region Users

        CreateMap<User, CreateUserDTO>();
        CreateMap<CreateUserDTO, User>();
        CreateMap<User, UserResponseDTO>();
        CreateMap<UserResponseDTO, User>();

        #endregion
    }
}