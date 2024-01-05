using Application.DTOs.Misc;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUnitService
    {
        Task CheckUnitExistence(int unitId);
        Task<UnitResponseDTO> GetUnitByIdAsync(int unitId);
    }
}
