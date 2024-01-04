using Application.DTOs;
using Application.DTOs.Device;
using Application.DTOs.DeviceType;
using Application.DTOs.Misc;
using Application.DTOs.Setting;
using Application.DTOs.SettingValue;
using Application.DTOs.UsersOnDevices;
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

        CreateMap<Device, UpdateDeviceDTO>();
        CreateMap<UpdateDeviceDTO, Device>();

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

        CreateMap<SettingValue, SettingValueResponseDTO>();
        CreateMap<SettingValueRequestDTO, SettingValue>();
        
        CreateMap<Setting, SettingResponseDTO>();
        CreateMap<SettingRequestDTO, Setting>();

        #endregion

        #region Misc

        CreateMap<Unit, UnitDTO>();
        CreateMap<UnitDTO, Unit>();

        CreateMap<DeviceType, LabelValueOptionDTO>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Name));

        #endregion

        #region UserOnDevices
        CreateMap<UsersOnDevices, UsersOnDevicesResponseDTO>();
        CreateMap<UsersOnDevicesResponseDTO, UsersOnDevices>();

        CreateMap<UsersOnDevices, CreateUserOnDeviceDTO>()
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.Device.Id));

        CreateMap<CreateUserOnDeviceDTO, UsersOnDevices>()
            .ForMember(dest => dest.Device, opt => opt.Ignore()); // Assuming you don't want to map back to the Device property
        #endregion
    }
}