using Application.ApplicationServices.Interfaces;
using Application.DTOs.UsersOnDevices;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class UsersOnDevicesService : IUsersOnDevicesService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UsersOnDevices> _userOnDeviceRepository;

        public UsersOnDevicesService(IMapper mapper, IRepository<UsersOnDevices> userOnDeviceRepository)
        {
            _mapper = mapper;
            _userOnDeviceRepository = userOnDeviceRepository;
        }

        public async Task<UsersOnDevicesResponseDTO> CreateUserOnDeviceAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO)
        {
            var newUsersOnDevices = await _userOnDeviceRepository.CreateAsync(_mapper.Map<UsersOnDevices>(createUserOnDeviceDTO));
            return _mapper.Map<UsersOnDevicesResponseDTO>(newUsersOnDevices);
        }

        public async Task<bool> DeleteUserOnDeviceAsync(int userOnDeviceId)
        {
            return await _userOnDeviceRepository.DeleteAsync(userOnDeviceId);  
        }

        public async Task<IEnumerable<UsersOnDevicesResponseDTO>> GetUsersOnDevicesByUserIdAsync(string userId)
        {
            var userOnDevices = await _userOnDeviceRepository.GetCollectionByConditionAsync(uod => uod.UserId == userId);

            return _mapper.Map<IEnumerable<UsersOnDevicesResponseDTO>>(userOnDevices);
        }
    }
}
