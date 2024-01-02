using Application.DTOs.Metrics;
using Domain.Enums;

namespace Application.ApplicationServices.Interfaces
{
    public interface IAggregatedLogsService
    {
        Task<IEnumerable<AggregatedLogsResponseDTO>> GetAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string startDate, string endDate);
    }
}
