using Application.DTOs.Misc;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUnitService
    {
        Task<UnitDTO> GetUnitByIdAsync(int unitId);
    }
}
