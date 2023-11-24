using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class FirmwareService : IFirmwareService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<FileSend> _firmwareRepository;
        public FirmwareService(IMapper mapper, IRepository<FileSend> firmwareRepository)
        {
            _mapper = mapper;
            _firmwareRepository = firmwareRepository;
        }

        public async Task<CreateFileSendDTO> CreateFileSendAsync(CreateFileSendDTO createFileSendDTO)
        {
            ValidateFileSendDTO(createFileSendDTO);

            Random rnd = new Random();
            int current = rnd.Next(1, 500);
            int total = current + rnd.Next(1, 500);

            FileSend newFileSend = new FileSend
            {
                DeviceId = createFileSendDTO.DeviceId, // needs some sort of validation
                UserId = createFileSendDTO.UserId, // needs some sort of validation
                UpdateStatus = UpdateStatus.New.ToString(),
                File = createFileSendDTO.File,
                CurrPart = current,
                TotParts = total
            };

            await _firmwareRepository.CreateAsync(newFileSend);

            return _mapper.Map<CreateFileSendDTO>(newFileSend);
        }

        public async Task<IEnumerable<FileSendResponseDTO>> GetFileSendHistoryByDeviceIdAsync(int deviceId)
        {
            var fileSends = await _firmwareRepository.GetAllAsync();

            var fileSendByDevice = fileSends.Where(f => f.DeviceId == deviceId);

            return _mapper.Map<IEnumerable<FileSendResponseDTO>>(fileSendByDevice);
        }

        private void ValidateFileSendDTO(CreateFileSendDTO fileSendDTO)
        {
            if (fileSendDTO == null)
                throw new BadRequestException("File DTO cannot be null");

            if (string.IsNullOrEmpty(fileSendDTO.File))
                throw new BadRequestException("File cannot be null or empty");

            if (fileSendDTO.UserId <= 0)
                throw new BadRequestException("User id cannot be negative or 0");

            if (fileSendDTO.DeviceId <= 0)
                throw new BadRequestException("Device id cannot be negative or 0");

            //TODO: check for file validation (maybe diff extensions)
        }
    }
}
