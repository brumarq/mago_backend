using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Bogus;
using Domain.Entities;

namespace Application.ApplicationServices
{
    public class DeviceTypeService : IDeviceTypeService
    {
        private readonly IMapper _mapper;
        private readonly IFakerService _fakerService;

        public DeviceTypeService(IMapper mapper, IFakerService fakerService)
        {
            _mapper = mapper;
            _fakerService = fakerService;
        }

        public async Task<IEnumerable<DeviceTypeResponseDTO>> GetDeviceTypesAsync()
        {
            var deviceTypes = await _fakerService.GetFakeDeviceTypesAsync();

            return _mapper.Map<IEnumerable<DeviceTypeResponseDTO>>(deviceTypes);
        }

        public async Task<CreateDeviceTypeDTO> CreateDeviceTypeAsync(CreateDeviceTypeDTO deviceType)
        {
            ValidateDeviceTypeDTO(deviceType);

            var deviceTypes = (await _fakerService.GetFakeDeviceTypesAsync()).ToList();

            DeviceType newDeviceType = new DeviceType
            {
                Id = deviceTypes.Count + 1,
                Name = deviceType.Name,
            };

            await _fakerService.CreateFakeDeviceTypeAsync(newDeviceType);

            return _mapper.Map<CreateDeviceTypeDTO>(deviceType);
        }

        public async Task<UpdateDeviceTypeDTO> UpdateDeviceTypeAsync(int id, UpdateDeviceTypeDTO deviceType)
        {
            ValidateDeviceTypeDTO(deviceType);

            var deviceTypes = (await _fakerService.GetFakeDeviceTypesAsync()).ToList();

            var singleDeviceType = deviceTypes.FirstOrDefault(dt => dt.Id == id);

            if (singleDeviceType == null)
                throw new BadRequestException($"Device type with id {id} does not exist");

            singleDeviceType.UpdatedAt = DateTime.Now;
            singleDeviceType.Name = deviceType.Name;

            await _fakerService.UpdateFakeDeviceTypeAsync(id, singleDeviceType);

            return _mapper.Map<UpdateDeviceTypeDTO>(deviceType);
        }

        private void ValidateDeviceTypeDTO(CreateDeviceTypeDTO deviceType)
        {
            if (deviceType == null)
                throw new BadRequestException("The object cannot be null");

            if (string.IsNullOrEmpty(deviceType.Name))
                throw new BadRequestException("The name property is required to be filled out");
        }
    }
}
