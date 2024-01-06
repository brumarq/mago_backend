using Application.DTOs.DeviceMetrics;
using Domain.Enums;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceMetricsService
    {
        Task<IEnumerable<DeviceMetricsResponseDTO>> GetDeviceMetricsAsync(int deviceId);
        Task<IEnumerable<DeviceAggregatedLogsResponseDTO>> GetDeviceAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string startDate, string endDate);
    }
}
