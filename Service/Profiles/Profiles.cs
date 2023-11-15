using AutoMapper;
using Model.DTOs;
using Model.DTOs.Users;
using Model.Entities;

namespace Service.Profiles
{
    public class Profiles : Profile
	{
		public Profiles()
		{
            #region Customers
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();
            #endregion

            #region Employees
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<EmployeeDTO, Employee>();
            #endregion

            #region Users
            CreateMap<User, CreateUserDTO>();
            CreateMap<CreateUserDTO, User>();
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserResponseDTO, User>();
            #endregion
        }
    }
}

