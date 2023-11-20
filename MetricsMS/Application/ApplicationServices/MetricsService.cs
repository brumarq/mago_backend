﻿using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.ApplicationServices
{
    public class MetricsService : IMetricsService
    {
        private readonly IMapper _mapper;
        private readonly IFakerService _fakerService;

        public MetricsService(IMapper mapper, IFakerService fakerService)
        {
            _mapper = mapper;
            _fakerService = fakerService;
        }

        public Task<IEnumerable<AggregatedLog>> GetAggregatedLogs(AggregatedLogDateType aggregatedLogDateType)
        {
            var aggregatedLogs = _fakerService.GetFakeAggregatedLogs();

            if (aggregatedLogs == null)
                throw new BadRequestException("Something went wrong while fetching the aggregated logs...");

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now); 

            switch (aggregatedLogDateType)
            {
                case AggregatedLogDateType.Weekly:
                    aggregatedLogs = aggregatedLogs.Where(ad => ad.Date >= currentDate.AddDays(-7)).ToList();
                    break;
                case AggregatedLogDateType.Monthly:
                    aggregatedLogs = aggregatedLogs.Where(ad => ad.Date >= currentDate.AddMonths(-1)).ToList();
                    break;
                case AggregatedLogDateType.Yearly:
                    aggregatedLogs = aggregatedLogs.Where(ad => ad.Date >= currentDate.AddYears(-1)).ToList();
                    break;
            }

            return Task.FromResult(aggregatedLogs);
        }

        public Task<IEnumerable<LogCollection>> GetDeviceMetrics(int deviceId)
        {
            var deviceMetrics = _fakerService.GetFakeDeviceMetrics();

            ValidateDevice(deviceId, deviceMetrics);

            var metricsByDeviceId = deviceMetrics.Where(dm => dm.DeviceId == deviceId);

            return Task.FromResult(metricsByDeviceId);
        }

        private void ValidateDevice(int deviceId, IEnumerable<LogCollection> deviceMetrics)
        {
            if (deviceId < 0)
                throw new BadRequestException("Device id cannot be 0 or negative");

            var containsId = deviceMetrics.Any(dm => dm.DeviceId == deviceId);

            if (!containsId)
                throw new NotFoundException($"Device with id {deviceId} is not a valid device!");
        }    
    }
}
