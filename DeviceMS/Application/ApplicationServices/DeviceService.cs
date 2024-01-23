using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.Exceptions;
using AutoMapper;
using Bogus.DataSets;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class DeviceService : IDeviceService
    {
        private readonly IMapper _mapper;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IRepository<Device> _repository;

        public DeviceService(IMapper mapper, IRepository<Device> repository, IDeviceTypeService deviceTypeService)
        {
            _mapper = mapper;
            _repository = repository;
            _deviceTypeService = deviceTypeService;
        }

        public async Task<DeviceResponseDTO> CreateDeviceAsync(CreateDeviceDTO newDeviceDto)
        {
            ValidateDevice(newDeviceDto);

            var selectedDeviceType = _mapper.Map<DeviceType>(await _deviceTypeService.GetDeviceTypeByIdAsync(newDeviceDto.DeviceTypeId));
            if (selectedDeviceType == null)
                throw new NotFoundException("The selected device type does not exist.");

            var newDevice = await _repository.CreateAsync(_mapper.Map<Device>(newDeviceDto));
            return _mapper.Map<DeviceResponseDTO>(newDevice);
        }

        public async Task<IEnumerable<DeviceResponseDTO>> GetDevicesAsync(int? pageNumber = null, int? pageSize = null)
        {
            var devices = await _repository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<DeviceResponseDTO>>(devices);
        }

        public async Task<DeviceResponseDTO> GetDeviceByIdAsync(int id)
        {
            var device = await _repository.GetByConditionAsync(d => d.Id == id);
            return _mapper.Map<DeviceResponseDTO>(device);
        }

        public async Task<bool?> UpdateDeviceAsync(int id, UpdateDeviceDTO updateDeviceDto)
        {
            ValidateDevice(updateDeviceDto);
            return await _repository.UpdateAsync(_mapper.Map<Device>(updateDeviceDto,
                opt => opt.AfterMap((src, dest) => dest.Id = id)));
        }

        private void ValidateDevice(DeviceRequestDTO device)
        {
            switch (device)
            {
                case null:
                    throw new BadRequestException("The object cannot be null.");

                case { Name: null } or { Name: "" }:
                    throw new BadRequestException("The 'Name' property is required to be filled out");

                case { DeviceTypeId: <= 0 }:
                    throw new BadRequestException("The 'DeviceTypeId' property cannot be negative or 0.");

                case { AuthId: null } or { AuthId: "" }:
                    throw new BadRequestException("The 'AuthId' property is required to be filled out");

                case { Password: null } or { Password: "" }:
                    throw new BadRequestException("The 'Password' property is required to be filled out");
            }
        }
    }
}