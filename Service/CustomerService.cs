using System;
using AutoMapper;
using DAL.Repository;
using Model.DTOs;
using Model.Entities;
using Service.Interfaces;

namespace Service
{
	public class CustomerService : ICustomerService
	{
        private readonly IMapper _mapper;
        private readonly IRepository<Customer> _repository;

		public CustomerService(IMapper mapper, IRepository<Customer> repository)
		{
            _mapper = mapper;
            _repository = repository;
		}

        public async Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDTO)
        {
            var customer = await _repository.CreateAsync(_mapper.Map<Customer>(customerDTO));
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _repository.GetByConditionAsync(c => c.Id == id);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<bool?> UpdateCustomerAsync(CustomerDTO customerDTO)
        {
            return await _repository.UpdateAsync(_mapper.Map<Customer>(customerDTO));
        }
    }
}

