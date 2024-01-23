using Application.DTOs.DeviceMetrics;
using Domain.Enums;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceAggregatedLogsService
    {
        Task<IEnumerable<DeviceAggregatedLogsResponseDTO>> GetDeviceAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string startDate, string endDate, int pageNumber, int pageSize);
    }
}
