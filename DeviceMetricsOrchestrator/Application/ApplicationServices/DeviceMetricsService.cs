using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Domain.Enums;

namespace Application.ApplicationServices
{
    public class DeviceMetricsService : IDeviceMetricsService
    {
        private readonly IMetricsService _metricsService;
        private readonly IDeviceService _deviceService;
        private readonly IUnitService _unitService;
        public DeviceMetricsService(IMetricsService metricsService, IDeviceService deviceService, IUnitService unitService)
        {
            _deviceService = deviceService;
            _metricsService = metricsService;
            _unitService = unitService;
        }       

        public async Task<IEnumerable<DeviceMetricsResponseDTO>> GetLastMetricsForDeviceAsync(int deviceId, int pageNumber, int pageSize)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            var metrics = await _metricsService.GetLatestMetricsForDeviceAsync(deviceId, pageNumber, pageSize);

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
