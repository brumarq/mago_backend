using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Bogus.DataSets;
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

        public async Task<IEnumerable<DeviceResponseDTO>> GetAllDevicesAsync()
        {
            var devices = await _fakerService.GetFakeDevicesAsync();

            return _mapper.Map<IEnumerable<DeviceResponseDTO>>(devices);
        }

        public async Task<DeviceResponseDTO> GetDeviceByIdAsync(int deviceId)
        {
            var devices = await _fakerService.GetFakeDevicesAsync();

            ValidateDevice(deviceId);

            var device = devices.FirstOrDefault(d => d.Id == deviceId);

            return _mapper.Map<DeviceResponseDTO>(device);
        }

        public async Task<CreateDeviceDTO> CreateDeviceAsync(CreateDeviceDTO createDeviceDTO)
        {
            var devices = (await _fakerService.GetFakeDevicesAsync()).ToList();

            Device newDevice = new Device();

            newDevice.Id = devices.Count + 1;
            newDevice.Name = createDeviceDTO.Name;

            newDevice.DeviceType = await GetDeviceTypeById(createDeviceDTO.DeviceTypeId);
            newDevice.SendSettingsAtConn = createDeviceDTO.SendSettingsAtConn;
            newDevice.SendSettingsNow = createDeviceDTO.SendSettingsNow;
            newDevice.Salt = newDevice.GenerateSalt();
            newDevice.PwHash = newDevice.GenerateHash(createDeviceDTO.Password);
            newDevice.AuthId = createDeviceDTO.AuthId;

            await _fakerService.CreateFakeDeviceAsync(newDevice);

            return _mapper.Map<CreateDeviceDTO>(createDeviceDTO);
        }

        private async void ValidateDevice(int deviceId)
        {
            if (deviceId <= 0)
                throw new BadRequestException("The device id cannot be 0 or negative.");

            var devices = await _fakerService.GetFakeDevicesAsync();

            if (!devices.Any(d => d.Id == deviceId))
                throw new NotFoundException($"The device with id {deviceId} does not exist.");
        }

        private async Task<DeviceType> GetDeviceTypeById(int deviceTypeId)
        {
            var deviceType = (await _fakerService.GetFakeDeviceTypesAsync()).FirstOrDefault(dt => dt.Id == deviceTypeId);

            if (deviceType == null)
                throw new BadRequestException($"Device type with id {deviceTypeId} does not exist");

            return deviceType;
        }
    }
}
