using Application.DTOs.Misc;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUnitService
    {
        Task<bool> UnitExistsAsync(int unitId);
        Task<UnitResponseDTO> GetUnitByIdAsync(int unitId);
    }
}
