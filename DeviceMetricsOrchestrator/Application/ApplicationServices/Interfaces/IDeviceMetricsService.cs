using Application.DTOs.DeviceMetrics;
using Domain.Enums;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceMetricsService
    {
        Task<IEnumerable<DeviceMetricsResponseDTO>> GetLastMetricsForDeviceAsync(int deviceId);
    }
}
