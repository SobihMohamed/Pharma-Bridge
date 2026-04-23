using AutoMapper;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.DTOs.Complaint;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.AutoMapper.ComplaintMapping
{
    public class AdminComplaintProfile : Profile
    {
        public AdminComplaintProfile()
        {
            CreateMap<Complaint, ComplaintDto>()
                .ForMember(dest => dest.SubmittedByName, opt => opt.MapFrom(src => src.SubmittedBy != null ? src.SubmittedBy.UserName : "Unknown"));
            
            //Mapping for the Details View (ComplaintDetailsDto)
            CreateMap<Complaint, ComplaintDetailsDto>()
                .IncludeBase<Complaint, ComplaintDto>()
                .ForMember(dest => dest.ResolvedByName, opt => opt.MapFrom(src => src.ResolvedBy != null ? src.ResolvedBy.UserName : "N/A"));
        }
    } 
}
