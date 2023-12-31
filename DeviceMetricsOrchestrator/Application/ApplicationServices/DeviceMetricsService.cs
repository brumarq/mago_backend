using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.DTOs.Metrics;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;

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

        public async Task<IEnumerable<DeviceAggregatedLogsResponseDTO>> GetDeviceAggregatedLogsAsync(AggregatedLogDateType aggregatedLogDateType, int deviceId, int fieldId)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(deviceId);
                var aggregatedLogs = await _aggregatedLogsService.GetAggregatedLogsAsync(aggregatedLogDateType, deviceId, fieldId);

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
            catch(Exception ex)
            {
                throw;
            }    
        }

        public async Task<IEnumerable<DeviceMetricsResponseDTO>> GetDeviceMetricsAsync(int deviceId)
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
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
