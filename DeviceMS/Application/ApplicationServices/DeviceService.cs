using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;

namespace Application.ApplicationServices
{
    public class DeviceService : IDeviceService
    {
        private readonly IMapper _mapper;
        private readonly IFakerService _fakerService;
        public DeviceService(IMapper mapper, IFakerService fakerService)
        {
            _mapper = mapper;
            _fakerService = fakerService;
        }
        public async Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId)
        {
            var devices = await _fakerService.GetFakeDevicesAsync();

            ValidateDevice(deviceId, devices);

            var device = devices.FirstOrDefault(d => d.Id == deviceId);

            return _mapper.Map<DeviceResponseDTO>(device);
        }


        private void ValidateDevice(int deviceId, IEnumerable<Device> devices)
        {
            if (deviceId <= 0)
                throw new BadRequestException("The device id cannot be 0 or negative.");

            if (!devices.Any(d => d.Id == deviceId))
                throw new NotFoundException($"The device with id {deviceId} does not exist.");
        }
    }
}
