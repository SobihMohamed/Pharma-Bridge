using AutoMapper;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.DTOs.PatientAddresses;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.AutoMapper.PatientMapping
{
    public class PatientProfileMapping : Profile
    {
        public PatientProfileMapping()
        {
            // Address Mapping
            CreateMap<PatientAddress, PatientAddressDto>();

            // PatientProfile → PatientProfileDto
            CreateMap<PatientProfile, PatientProfileDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.ApplicationUser.FullName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.ApplicationUser.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.ApplicationUser.PhoneNumber));

            // PatientProfile → PatientProfileDetailsDto
            CreateMap<PatientProfile, PatientProfileDetailsDto>()
                .IncludeBase<PatientProfile, PatientProfileDto>()
                .ForMember(dest => dest.Addresses,
                    opt => opt.MapFrom(src => src.PatientAddresses))
                .ForMember(dest => dest.TotalPrescriptionRequests,
                    opt => opt.MapFrom(src => src.PrescriptionRequests.Count))
                .ForMember(dest => dest.OrdersCount,
                    opt => opt.MapFrom(src => src.Orders.Count))
                .ForMember(dest => dest.ComplaintsSubmitted,
                    opt => opt.MapFrom(src => src.ApplicationUser.Complaints.Count))
                .ForMember(dest => dest.TotalPharmacyRatings,
                    opt => opt.MapFrom(src => src.PharmacyRatings.Count))
                .ForMember(dest => dest.PendingOrders,
                    opt => opt.MapFrom(src =>
                        src.Orders.Count(o => o.OrderStatus == OrderStatus.Pending)))
                .ForMember(dest => dest.CompletedOrders,
                    opt => opt.MapFrom(src =>
                        src.Orders.Count(o => o.OrderStatus == OrderStatus.Completed)))
                .ForMember(dest => dest.CancelledOrders,
                    opt => opt.MapFrom(src =>
                        src.Orders.Count(o => o.OrderStatus == OrderStatus.Cancelled)));
                    }
    }
}