using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.UsersOnDevices;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class UsersOnDevicesService : IUsersOnDevicesService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UsersOnDevices> _userOnDeviceRepository;
        private readonly IDeviceService _deviceService;

        public UsersOnDevicesService(IMapper mapper, IRepository<UsersOnDevices> userOnDeviceRepository, IDeviceService deviceService)
        {
            _mapper = mapper;
            _userOnDeviceRepository = userOnDeviceRepository;
            _deviceService = deviceService;
        }

        public async Task<CreateUserOnDeviceDTO> CreateUserOnDeviceAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            ValidateCreateUserOnDeviceDTO(createUserOnDeviceDTO); // add more validation later

            var selectedDevice = _mapper.Map<Device>(await _deviceService.GetDeviceByIdAsync(createUserOnDeviceDTO.DeviceId));

            if (selectedDevice == null)
                throw new NotFoundException("The selected device does not exist.");

            var existingUserOnDevice = await _userOnDeviceRepository.GetByConditionAsync(uod => uod.UserId == createUserOnDeviceDTO.UserId && uod.Device.Id == createUserOnDeviceDTO.DeviceId);

            if (existingUserOnDevice != null)
                throw new BadRequestException($"The user {createUserOnDeviceDTO.UserId} is already assigned to device {createUserOnDeviceDTO.DeviceId}");

            var newUserOnDevice = await _userOnDeviceRepository.CreateAsync(_mapper.Map<UsersOnDevices>(createUserOnDeviceDTO));

            return _mapper.Map<CreateUserOnDeviceDTO>(newUserOnDevice);
        }

        public async Task<bool> DeleteUserOnDeviceAsync(int id)
        {
            return await _userOnDeviceRepository.DeleteAsync(id);  
        }

        public async Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId)
        {
            var userOnDevices = await _userOnDeviceRepository.GetCollectionByConditionAsync(uod => uod.UserId == userId);

            return _mapper.Map<IEnumerable<UsersOnDevicesResponseDTO>>(userOnDevices);
        }

        public async Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByDeviceIdAsync(int deviceId)
        {
            var userOnDevices = await _userOnDeviceRepository.GetCollectionByConditionAsync(uod => uod.DeviceId == deviceId);

            return _mapper.Map<IEnumerable<UsersOnDevicesResponseDTO>>(userOnDevices);
        }

        private void ValidateCreateUserOnDeviceDTO(CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            if (string.IsNullOrEmpty(createUserOnDeviceDTO.UserId))
                throw new BadRequestException("The user id field is required to be filled out.");

            if (createUserOnDeviceDTO.DeviceId <= 0)
                throw new BadRequestException("The device id cannot be 0 or negative");
        }
    }
}
