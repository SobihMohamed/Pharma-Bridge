using AutoMapper;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.AutoMapper.PharmacyMapping
{
    public class PharmacyProfileMapping : Profile
    {
        public PharmacyProfileMapping()
        {
            CreateMap<PharmacyToCreateDto, Pharmacy>();

            CreateMap<Pharmacy, PharmacyDetailsDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PharmaOwner.ApplicationUser.FullName))
                .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => src.PharmaOwner.ApplicationUser.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime.HasValue ? src.OpenTime.Value.ToString("HH:mm") : null))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime.HasValue ? src.CloseTime.Value.ToString("HH:mm") : null));

            CreateMap<Pharmacy, PharmacyDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime.HasValue ? src.OpenTime.Value.ToString("HH:mm") : null))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime.HasValue ? src.CloseTime.Value.ToString("HH:mm") : null));
            
            CreateMap<PharmacyToUpdateDto, Pharmacy>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.GeneralArea))
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
