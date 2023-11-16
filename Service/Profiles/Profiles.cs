using AutoMapper;
using Model.DTOs;
using Model.DTOs.Users;
using Model.Entities;
using Model.Entities.Users;

namespace Service.Profiles
{
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
}

