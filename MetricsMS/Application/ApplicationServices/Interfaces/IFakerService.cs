using Domain.Entities;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFakerService
    {
        Task<IEnumerable<LogCollection>> GetFakeDeviceMetricsAsync();
        Task<IEnumerable<AggregatedLog>> GetFakeAggregatedLogsAsync();
    }
}
