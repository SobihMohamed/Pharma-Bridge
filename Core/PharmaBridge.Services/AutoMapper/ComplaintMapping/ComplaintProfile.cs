using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.AutoMapper.ComplaintMapping
{
    public class ComplaintProfile : Profile
    {
        public ComplaintProfile()
        {
            CreateMap<CreateComplaintDto, Complaint>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedById, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ResolvedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.ResolvedById, opt => opt.Ignore());
        }   
    }
}
