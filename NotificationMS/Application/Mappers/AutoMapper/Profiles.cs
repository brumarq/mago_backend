using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers.AutoMapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Status, CreateNotificationDTO>();
            CreateMap<CreateNotificationDTO, Status>();
            CreateMap<Status, NotificationResponseDTO>();
            CreateMap<NotificationResponseDTO, Status>();
            CreateMap<StatusType, StatusTypeDTO>();
            CreateMap<StatusTypeDTO,  StatusType>();
            CreateMap<NotificationTokenOnUserDTO, NotificationTokenOnUser>();
            CreateMap<NotificationTokenOnUser, NotificationTokenOnUserDTO>();
        }
    }
}
