using Application.ApplicationServices;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers.AutoMapper;

public class Profiles : Profile
{
    public Profiles()
    {
        #region Devices
        CreateMap<Device, DeviceResponseDTO>();
        CreateMap<DeviceResponseDTO, Device>();
        #endregion

        #region DeviceTypes
        CreateMap<DeviceType, DeviceTypeResponseDTO>();
        CreateMap<DeviceTypeResponseDTO, DeviceType>();

        CreateMap<DeviceType, CreateDeviceTypeDTO>();
        CreateMap<CreateDeviceTypeDTO, DeviceType>();

        CreateMap<DeviceType, UpdateDeviceTypeDTO>();
        CreateMap<UpdateDeviceTypeDTO, DeviceType>();
        #endregion
    }
}