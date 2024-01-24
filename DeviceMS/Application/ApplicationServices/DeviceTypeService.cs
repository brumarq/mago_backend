using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.DeviceType;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class DeviceTypeService : IDeviceTypeService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<DeviceType> _repository;

        public DeviceTypeService(IMapper mapper, IRepository<DeviceType> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<DeviceTypeResponseDTO> CreateDeviceTypeAsync(CreateDeviceTypeDTO newDeviceTypeDto)
        {
            ValidateDeviceTypeDto(newDeviceTypeDto);
            var newDeviceType = await _repository.CreateAsync(_mapper.Map<DeviceType>(newDeviceTypeDto));
            return _mapper.Map<DeviceTypeResponseDTO>(newDeviceType);
        }

        public async Task<IEnumerable<DeviceTypeResponseDTO>> GetDeviceTypesAsync(int? pageNumber = null, int? pageSize = null)
        {
            var deviceTypes = await _repository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<DeviceTypeResponseDTO>>(deviceTypes);
        }

        public async Task<DeviceTypeResponseDTO> GetDeviceTypeByIdAsync(int id)
        {
            var deviceType = await _repository.GetByConditionAsync(dt => dt.Id == id);
            return _mapper.Map<DeviceTypeResponseDTO>(deviceType);
        }

        public async Task<bool?> UpdateDeviceTypeAsync(int id, UpdateDeviceTypeDTO updatedDeviceTypeDto)
        {
            ValidateDeviceTypeDto(updatedDeviceTypeDto);
            return await _repository.UpdateAsync(_mapper.Map<DeviceType>(updatedDeviceTypeDto,
                opt => opt.AfterMap((src, dest) => dest.Id = id)));
        }

        public async Task<IEnumerable<LabelValueOptionDTO>> GetDeviceTypeDropdown(int? pageNumber = null, int? pageSize = null)
        {
            var deviceTypes = await _repository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<LabelValueOptionDTO>>(deviceTypes);
        }


        private void ValidateDeviceTypeDto(DeviceTypeRequestDTO deviceType)
        {
            if (deviceType == null)
                throw new BadRequestException("The object cannot be null");

            if (string.IsNullOrEmpty(deviceType.Name))
                throw new BadRequestException("The name property is required to be filled out");
        }
    }
}