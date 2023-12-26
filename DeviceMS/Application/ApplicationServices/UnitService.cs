using Application.ApplicationServices.Interfaces;
using Application.DTOs.Misc;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class UnitService : IUnitService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Unit> _unitRepository;
        public UnitService(IMapper mapper, IRepository<Unit> unitRepository)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }
        public async Task<UnitDTO> GetUnitByIdAsync(int unitId)
        {
            var unit = await _unitRepository.GetByConditionAsync(u => u.Id == unitId);
            return _mapper.Map<UnitDTO>(unit);
        }
    }
}
