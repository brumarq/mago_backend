using Application.DTOs.Device;
using Application.DTOs.Metrics;

namespace Application.ApplicationServices.Interfaces;

public interface IMetricsService
{
    Task<IEnumerable<MetricsResponseDTO>> GetMetricsForDevice(int deviceId);
}