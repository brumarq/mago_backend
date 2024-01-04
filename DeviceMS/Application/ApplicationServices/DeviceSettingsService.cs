using Application.ApplicationServices.Interfaces;
using Application.DTOs.Setting;
using Application.DTOs.SettingValue;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class DeviceSettingsService : IDeviceSettingsService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SettingValue> _repository;

        public DeviceSettingsService(IMapper mapper, IRepository<SettingValue> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IEnumerable<SettingValueResponseDTO>> GetSettingsForDeviceAsync(int deviceId)
        {
            var deviceSettings = await _repository.GetCollectionByConditionAsync(sv => sv.Device.Id == deviceId);

            if (!deviceSettings.Any())
                throw new NotFoundException($"No settings found for Device with ID: {deviceId}");

            return _mapper.Map<IEnumerable<SettingValueResponseDTO>>(deviceSettings);
        }

        public async Task<SettingValueResponseDTO> AddSettingToDevice(CreateSettingValueDTO newSettingValueDto)
        {
            ValidateSettingValue(newSettingValueDto);

            var newSettingValue = await _repository.CreateAsync(_mapper.Map<SettingValue>(newSettingValueDto));
            return _mapper.Map<SettingValueResponseDTO>(newSettingValue);
        }

        // public async Task<bool?> UpdateSettingAsync(int id, UpdateSettingValueDTO updateSettingValueDto)
        // {
        //     ValidateSettingValue(updateSettingValueDto);
        //     return await _repository.UpdateAsync(_mapper.Map<SettingValue>(updateSettingValueDto,
        //         opt => opt.AfterMap((src, dest) => dest.Id = id)));
        // }

        // public async Task<bool> DeleteSettingFromDeviceAsync(int id)
        // {
        //     return await _repository.DeleteAsync(id);
        // }


        private void ValidateSettingValue(SettingValueRequestDTO newSettingValue)
        {
            if (newSettingValue.Value <= 0)
            {
                throw new BadRequestException("Value cannot be negative or 0"); // TODO: adjust check to requirement
            }

            if (newSettingValue.DeviceId <= 0)
            {
                throw new BadRequestException("Device id cannot be negative or 0");
            }

            if (string.IsNullOrEmpty(newSettingValue.UserId))
            {
                throw new BadRequestException("User id must be specified");
            }

            if (newSettingValue.Setting == null)
            {
                throw new BadRequestException("Setting cannot be null");
            }

            if (string.IsNullOrEmpty(newSettingValue.Setting.Name))
            {
                throw new BadRequestException("Setting Error: Name not specified");
            }

            if (newSettingValue.Setting.DefaultValue <= 0)
            {
                throw new BadRequestException("Setting Error: DefaultValue not specified or negative");
            }

            if (newSettingValue.Setting.UnitId <= 0)
            {
                throw new BadRequestException("Setting Error: UnitId not specified or negative");
            }
        }
    }
}