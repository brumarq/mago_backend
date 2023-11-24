using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.Setting;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;

namespace Application.ApplicationServices
{
    public class DeviceSettingsService : IDeviceSettingsService
    {
        private readonly IMapper _mapper;
        private readonly IFakerService _fakerService;

        public DeviceSettingsService(IMapper mapper, IFakerService fakerService)
        {
            _mapper = mapper;
            _fakerService = fakerService;
        }

        public async Task<IEnumerable<SettingValueResponseDTO>> GetSettingsForDeviceAsync(int deviceId)
        {
            var deviceSettings = await _fakerService.GetFakeSettingValuesAsync();

            if (deviceId <= 0)
                throw new BadRequestException("Device id cannot be negative or 0");

            var hasSettingsForDeviceId = deviceSettings.Any(ds => ds.Device.Id == deviceId);

            if (!hasSettingsForDeviceId)
                throw new BadRequestException($"No settings for the following device id: {deviceId}");

            var deviceSettingsForDevice = deviceSettings.Where(ds => ds.Device.Id == deviceId);

            return _mapper.Map<IEnumerable<SettingValueResponseDTO>>(deviceSettingsForDevice);
        }

        public async Task<SettingValueResponseDTO> AddSettingsToDevice(CreateSettingValueDTO newSettingValue)
        {
            ValidateSettingValueDTO(newSettingValue);

            var mappedSettingValue = _mapper.Map<SettingValue>(newSettingValue);
            
            var createdSettingValue = await _fakerService.CreateFakeSettingValueAsync(mappedSettingValue);
            return _mapper.Map<SettingValueResponseDTO>(createdSettingValue);
        }

        private void ValidateSettingValueDTO(CreateSettingValueDTO newSettingValue)
        {
            switch (newSettingValue)
            {
                case { Value: <= 0 }:
                    throw new BadRequestException("Value cannot be negative or 0"); // TODO: adjust check to requirement

                case { DeviceId: <= 0 }:
                    throw new BadRequestException("Device id cannot be negative or 0");

                case { UserId: <= 0 }:
                    throw new BadRequestException("User id cannot be negative or 0");

                case { Setting: null }:
                    throw new BadRequestException("Setting cannot be null");

                case { Setting.Name.Length: <= 0 }:
                    throw new BadRequestException("Setting Error: Name not specified");

                case { Setting.DefaultValue: <= 0 }:
                    throw new BadRequestException("Setting Error: DefaultValue not specified");

                case { Setting.UnitId: <= 0 }:
                    throw new BadRequestException("Setting Error: UnitId not specified");
            }
        }
    }
}