using System;
using AutoMapper;
using Model.DTOs;
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
        }
    }
}

