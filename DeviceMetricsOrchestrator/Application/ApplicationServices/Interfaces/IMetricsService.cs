using Application.DTOs.Metrics;

namespace Application.ApplicationServices.Interfaces;

public interface IMetricsService
{
    Task<IEnumerable<MetricsResponseDTO>> GetMetricsForDeviceAsync(int deviceId);
}