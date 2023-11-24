using Application.DTOs;
using Domain.Enums;

namespace Application.ApplicationServices.Interfaces
{
    public interface IMetricsService
    {
        Task<IEnumerable<DeviceMetricsResponseDTO>> GetDeviceMetricsAsync(int deviceId);
        Task<IEnumerable<AggregatedLogsResponseDTO>> GetAggregatedLogsAsync(AggregatedLogDateType dateFrequency);
    }
}
