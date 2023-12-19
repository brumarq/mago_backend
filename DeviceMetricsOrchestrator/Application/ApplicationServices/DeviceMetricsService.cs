using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.DTOs.Metrics;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Enums;
using System.Formats.Asn1;
using System.Globalization;

namespace Application.ApplicationServices
{
    public class DeviceMetricsService : IDeviceMetricsService
    {
        private readonly IMetricsService _metricsService;
        private readonly IAggregatedLogsService _aggregatedLogsService;
        private readonly IDeviceService _deviceService;
        public DeviceMetricsService(IMetricsService metricsService, IAggregatedLogsService aggregatedLogsService, IDeviceService deviceService)
        {
            _deviceService = deviceService;
            _metricsService = metricsService;
            _aggregatedLogsService = aggregatedLogsService;
        }

        public async Task<IEnumerable<DeviceAggregatedLogsResponseDTO>> GetDeviceAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            var aggregatedLogs = await _aggregatedLogsService.GetAggregatedLogsAsync(aggregatedLogDateType, deviceId, fieldId);

            return aggregatedLogs.Select(aggregatedLog => new DeviceAggregatedLogsResponseDTO
            {
                Id = aggregatedLog.Id,
                CreatedAt = aggregatedLog.CreatedAt,
                UpdatedAt = aggregatedLog.UpdatedAt,
                AverageValue = aggregatedLog.AverageValue,
                MinValue = aggregatedLog.MinValue,
                MaxValue = aggregatedLog.MaxValue,
                Device = device,
                Field = new FieldDTONew
                {
                    Id = aggregatedLog.Field!.Id,
                    CreatedAt = aggregatedLog.Field.CreatedAt,
                    UpdatedAt = aggregatedLog.Field.UpdatedAt,
                    Name = aggregatedLog.Field.Name,
                    UnitId = aggregatedLog.Field.UnitId,
                    DeviceType = device.DeviceType,
                    Loggable = aggregatedLog.Field.Loggable,
                }
            }).ToList();
        }

        public async Task<IEnumerable<DeviceMetricsResponseDTO>> GetDeviceMetricsAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            var metrics = await _metricsService.GetMetricsForDeviceAsync(deviceId);

            return metrics.Select(metric => new DeviceMetricsResponseDTO
            {
                Id = metric.Id,
                CreatedAt = metric.CreatedAt,
                UpdatedAt = metric.UpdatedAt,
                Value = metric.Value,
                Field = new FieldDTONew
                {
                    Id = metric.Field!.Id,
                    CreatedAt = metric.Field.CreatedAt,
                    UpdatedAt = metric.Field.UpdatedAt,
                    Name = metric.Field.Name,
                    UnitId = metric.Field.UnitId,
                    DeviceType = device.DeviceType,
                    Loggable = metric.Field.Loggable,
                },
                LogCollection = new LogCollectionDTONew
                {
                    Id = metric.LogCollection!.Id,
                    CreatedAt = metric.LogCollection.CreatedAt,
                    UpdatedAt = metric.LogCollection.UpdatedAt,
                    Device = device,
                    LogCollectionType = metric.LogCollection.LogCollectionType
                }
            }).ToList();
        }

        public async Task<string> ExportDeviceAggregatedLogsAsnc(ExportAggregatedLogsCsvDTO exportAggregatedLogsCsvDTO)
        {
            var aggregatedLogs = await GetDeviceAggregatedLogsAsync(exportAggregatedLogsCsvDTO.AggregatedLogDateType, exportAggregatedLogsCsvDTO.DeviceId, exportAggregatedLogsCsvDTO.FieldId);

            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Write header
                csv.WriteRecords(new List<DeviceAggregatedLogsResponseDTO> { new DeviceAggregatedLogsResponseDTO() });

                // Write records
                csv.WriteRecords(aggregatedLogs);

                return writer.ToString();
            }
        }
    }
}
