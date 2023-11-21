using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers.AutoMapper;

public class Profiles : Profile
{
    public Profiles()
    {
        #region Metrics
        CreateMap<LogCollection, DeviceMetricsResponseDTO>();
        CreateMap<DeviceMetricsResponseDTO, LogCollection>();
        CreateMap<AggregatedLog, AggregatedLogsResponseDTO>();
        CreateMap<AggregatedLogsResponseDTO, AggregatedLog>();
        #endregion
    }
}