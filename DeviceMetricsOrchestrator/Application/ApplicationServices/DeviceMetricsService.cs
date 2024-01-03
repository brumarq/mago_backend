using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Domain.Enums;

namespace Application.ApplicationServices
{
    public class DeviceMetricsService : IDeviceMetricsService
    {
        private readonly IMetricsService _metricsService;
        private readonly IAggregatedLogsService _aggregatedLogsService;
        private readonly IDeviceService _deviceService;
        private readonly IUnitService _unitService;
        public DeviceMetricsService(IMetricsService metricsService, IAggregatedLogsService aggregatedLogsService, IDeviceService deviceService, IUnitService unitService)
        {
            _deviceService = deviceService;
            _metricsService = metricsService;
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
                    }
                };

                responseList.Add(deviceAggregatedLogsResponse);
            }
            return responseList;
        }

        public async Task<IEnumerable<DeviceMetricsResponseDTO>> GetDeviceMetricsAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            var metrics = await _metricsService.GetMetricsForDeviceAsync(deviceId);

            var responseList = new List<DeviceMetricsResponseDTO>();

            foreach (var metric in metrics)
            {
                var field = metric.Field!;
                var unit = await _unitService.GetUnitByIdAsync(field.UnitId);

                var deviceMetricsResponse = new DeviceMetricsResponseDTO
                {
                    Id = metric.Id,
                    CreatedAt = metric.CreatedAt,
                    UpdatedAt = metric.UpdatedAt,
                    Value = metric.Value,
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
                    LogCollection = new LogCollectionResponseDTO
                    {
                        Id = metric.LogCollection!.Id,
                        CreatedAt = metric.LogCollection.CreatedAt,
                        UpdatedAt = metric.LogCollection.UpdatedAt,
                        Device = device,
                        LogCollectionType = metric.LogCollection.LogCollectionType
                    }
                };

                responseList.Add(deviceMetricsResponse);
            }

            return responseList;
        }
    }
}
