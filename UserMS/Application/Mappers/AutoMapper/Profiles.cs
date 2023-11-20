using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers.AutoMapper;

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