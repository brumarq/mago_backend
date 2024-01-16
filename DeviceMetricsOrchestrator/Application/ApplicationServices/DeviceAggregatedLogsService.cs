using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Domain.Enums;

namespace Application.ApplicationServices
{
    public class DeviceAggregatedLogsService : IDeviceAggregatedLogsService
    {
        private readonly IAggregatedLogsService _aggregatedLogsService;
        private readonly IDeviceService _deviceService;
        private readonly IUnitService _unitService;
        public DeviceAggregatedLogsService(IAggregatedLogsService aggregatedLogsService, IDeviceService deviceService, IUnitService unitService)
        {
            _deviceService = deviceService;
            _aggregatedLogsService = aggregatedLogsService;
            _unitService = unitService;
        }
        public async Task<IEnumerable<DeviceAggregatedLogsResponseDTO>> GetDeviceAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId, string startDate, string endDate)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            var aggregatedLogs = await _aggregatedLogsService.GetAggregatedLogsAsync(aggregatedLogDateType, deviceId, fieldId, startDate, endDate);

            var responseList = new List<DeviceAggregatedLogsResponseDTO>();

            foreach (var aggregatedLog in aggregatedLogs)
            {
                var field = aggregatedLog.Field!;
                var unit = await _unitService.GetUnitByIdAsync(field.UnitId);

                var deviceAggregatedLogsResponse = new DeviceAggregatedLogsResponseDTO
                {
                    Id = aggregatedLog.Id,
                    CreatedAt = aggregatedLog.CreatedAt,
                    UpdatedAt = aggregatedLog.UpdatedAt,
                    AverageValue = aggregatedLog.AverageValue,
                    MinValue = aggregatedLog.MinValue,
                    MaxValue = aggregatedLog.MaxValue,
                    Device = device,
                    Field = new FieldResponseDTO
                    {
                        Id = field.Id,
                        CreatedAt = field.CreatedAt,
                        UpdatedAt = field.UpdatedAt,
                        Name = field.Name,
                        Unit = unit, // Use the unit retrieved above
                        DeviceType = device.DeviceType,
                        Loggable = field.Loggable,
                    },
                    ReferenceDate = aggregatedLog.ReferenceDate
                };

                responseList.Add(deviceAggregatedLogsResponse);
            }
            return responseList;
        }
    }
}
