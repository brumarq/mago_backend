using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;

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
        public async Task<IEnumerable<DeviceSettingsResponseDTO>> GetSettingsForDevice(int deviceId)
        {
            var deviceSettings = await _fakerService.GetFakeSettingValues();

            if (deviceId <= 0)
                throw new BadRequestException("Device id cannot be negative or 0");

            var hasSettingsForDeviceId = deviceSettings.Any(ds => ds.Device.Id == deviceId);

            if (!hasSettingsForDeviceId)
                throw new BadRequestException($"No settings for the following device id: {deviceId}");

            var deviceSettingsForDevice = deviceSettings.Where(ds => ds.Device.Id == deviceId);

            return _mapper.Map<IEnumerable<DeviceSettingsResponseDTO>>(deviceSettingsForDevice);
        }
    }
}
