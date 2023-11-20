using Domain.Entities;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFakerService
    {
        IEnumerable<LogCollection> GetFakeDeviceMetrics();
        IEnumerable<AggregatedLog> GetFakeAggregatedLogs();
    }
}
