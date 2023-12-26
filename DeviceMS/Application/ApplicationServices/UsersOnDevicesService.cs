using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.DeviceType;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices
{
    public class UsersOnDevicesService : IUsersOnDevicesService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UsersOnDevices> _repository;

        public UsersOnDevicesService(IMapper mapper, IRepository<UsersOnDevices> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<UserOnDeviceResponseDTO> CreateUserOnDeviceAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            var newUsersOnDevices = await _repository.CreateAsync(_mapper.Map<UsersOnDevices>(createUserOnDeviceDTO));
            return _mapper.Map<UserOnDeviceResponseDTO>(newUsersOnDevices);
        }

        public async Task<bool> DeleteUserOnDeviceAsync(int userOnDeviceId)
        {
            return await _repository.DeleteAsync(userOnDeviceId);  
        }
    }
}
