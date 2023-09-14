using System;
using System.Linq.Expressions;
using Model.DTOs;
using Model.Entities;

namespace Service.Interfaces
{
	public interface ICustomerService
	{
        Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customer);
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerDTO> GetCustomerByIdAsync(int id);
        Task<bool?> UpdateCustomerAsync(CustomerDTO customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}