using System;
using Model.DTOs;
using Model.Entities;

namespace Service.Interfaces
{
	public interface IEmployeeService
	{
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employee);
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id);
        Task<bool?> UpdateEmployeeAsync(EmployeeDTO employee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}

