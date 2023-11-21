using Application.DTOs;
using AutoMapper;
using Bogus;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices
{
    public class FirmwareService : IFirmwareService
    {
        private readonly IMapper _mapper;
        private static List<FileSend> fileSends = GenerateFakeFileSends(10);
        public FirmwareService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<CreateFileSendDTO> CreateFileSendAsync(CreateFileSendDTO createFileSendDTO)
        {
            Random rnd = new Random();
            int parts = rnd.Next(1, 500);

            FileSend newFileSend = new FileSend
            {
                Id = fileSends.Count + 1,
                DeviceId = createFileSendDTO.DeviceId,
                UserId = 1,
                UpdateStatus = "SENT",
                File = createFileSendDTO.File,
                CurrPart = parts,
                TotParts = parts
            };

            fileSends.Add(newFileSend);

            return Task.FromResult(_mapper.Map<CreateFileSendDTO>(newFileSend));
        }

        public Task<IEnumerable<FileSendResponseDTO>> GetFileSendHistoryByDeviceId(int deviceId)
        {
            IEnumerable<FileSend> fileSendsByDevice = fileSends.Where(f => f.DeviceId == deviceId);

            var fileSendDTOs = _mapper.Map<IEnumerable<FileSendResponseDTO>>(fileSendsByDevice);

            return Task.FromResult(fileSendDTOs);
        }


        private static List<FileSend> GenerateFakeFileSends(int count)
        {
            Random rnd = new Random();

            var faker = new Faker<FileSend>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.UpdateStatus, f => "SENT")
                .RuleFor(u => u.DeviceId, f => rnd.Next(1, 4))
                .RuleFor(u => u.UserId, f => rnd.Next(1, 3))
                .RuleFor(u => u.File, f => f.Lorem.Word() + ".bin")
                .RuleFor(u => u.CurrPart, f => rnd.Next(1, 500))
                .RuleFor(u => u.TotParts, f => rnd.Next(1, 500));
                
            return faker.Generate(count);
        }
    }
}
