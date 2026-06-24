using AutoMapper;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using PharmaBridge.Shared.DTOs.PatientAddresses;

namespace PharmaBridge.Services.AutoMapper.PatientMapping
{
    public class PatientProfileMapping : Profile
    {
        public PatientProfileMapping()
        {
            //Address Mapping
            CreateMap<PatientAddress, PatientAddressDto>();

            //PatientProfile → PatientProfileDto
            CreateMap<PatientProfile, PatientProfileDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.ApplicationUser.FullName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.ApplicationUser.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.ApplicationUser.PhoneNumber));

            //PatientProfile → PatientProfileDetailsDto
            CreateMap<PatientProfile, PatientProfileDetailsDto>()
                .IncludeBase<PatientProfile, PatientProfileDto>()
                .ForMember(dest => dest.Addresses,
                    opt => opt.MapFrom(src => src.PatientAddresses))
                .ForMember(dest => dest.TotalPrescriptionRequests,
                    opt => opt.MapFrom(src => src.PrescriptionRequests.Count))
                .ForMember(dest => dest.OrdersCount,
                    opt => opt.MapFrom(src => src.Orders.Count))
                .ForMember(dest => dest.ComplaintsSubmitted,
                    opt => opt.MapFrom(src => src.ApplicationUser.Complaints.Count));
        }
    }
}