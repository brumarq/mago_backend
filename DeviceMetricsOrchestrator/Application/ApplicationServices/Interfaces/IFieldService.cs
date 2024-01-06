using Application.DTOs.DeviceMetrics;
using Application.DTOs.Metrics;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFieldService
    {
        Task<Dictionary<string,string>> CreateFieldAsync(CreateFieldDTO createFieldDTO);
    }
}
