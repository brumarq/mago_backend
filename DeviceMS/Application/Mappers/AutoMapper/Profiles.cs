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

        CreateMap<Device, CreateDeviceDTO>();
        CreateMap<CreateDeviceDTO, Device>();
        #endregion

        #region DeviceTypes
        CreateMap<DeviceType, DeviceTypeResponseDTO>();
        CreateMap<DeviceTypeResponseDTO, DeviceType>();

        CreateMap<DeviceType, CreateDeviceTypeDTO>();
        CreateMap<CreateDeviceTypeDTO, DeviceType>();

        CreateMap<DeviceType, UpdateDeviceTypeDTO>();
        CreateMap<UpdateDeviceTypeDTO, DeviceType>();
        #endregion

        #region Device Settings
        CreateMap<SettingValue, DeviceSettingsResponseDTO>();
        CreateMap<DeviceSettingsResponseDTO, SettingValue>();
        #endregion
    }
}