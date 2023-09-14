using System;
using AutoMapper;
using DAL.Repository;
using Model.DTOs;
using Model.Entities;
using Service.Interfaces;

namespace Service
{
	public class EmployeeService : IEmployeeService
	{
        private readonly IMapper _mapper;
        private readonly IRepository<Employee> _repository;

		public EmployeeService(IMapper mapper, IRepository<Employee> repository)
		{
            _mapper = mapper;
            _repository = repository;
		}

        public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDTO)
        {
            var employee = await _repository.CreateAsync(_mapper.Map<Employee>(employeeDTO));
            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var employees = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDTO>>(employees);
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int id)
        {
            var employee = await _repository.GetByConditionAsync(c => c.Id == id);
            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<bool?> UpdateEmployeeAsync(EmployeeDTO employeeDTO)
        {
            return await _repository.UpdateAsync(_mapper.Map<Employee>(employeeDTO));
        }
    }
}

