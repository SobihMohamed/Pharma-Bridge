using AutoMapper;
using PharmaBridge.Shared.DTOs.Notificaiton;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.AutoMapper.NotificationMapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Domain.Models.UserAccess.Notification, NotificationDto>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Description));
        }
    }
}
