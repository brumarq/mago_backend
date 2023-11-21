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
            CreateMap<FileSend, CreateFileSendDTO>();
            CreateMap<CreateFileSendDTO, FileSend>();
            CreateMap<FileSend, FileSendResponseDTO>();
            CreateMap<FileSendResponseDTO, FileSend>();
        }
    }
}
