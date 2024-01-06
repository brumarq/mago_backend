using Application.DTOs.DeviceMetrics;
using Application.DTOs.Metrics;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFieldService
    {
        Task<CreateFieldDTO> CreateFieldAsync(CreateFieldDTO createFieldDTO);
    }
}
