using Domain.Entities;
using Domain.Enums;

namespace Application.ApplicationServices.Interfaces
{
    public interface IMetricsService
    {
        Task<IEnumerable<LogCollection>> GetDeviceMetrics(int deviceId);
        Task<IEnumerable<AggregatedLog>> GetAggregatedLogs(AggregatedLogDateType dateFrequency);
    }
}
