using System;
using AutoMapper;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Services.Resolver;

namespace PharmaBridge.Services.AutoMapper.PharmacyMapping
{
    public class PharmacyProfileMapping : Profile
    {
        public PharmacyProfileMapping()
        {
            // Create Mapping
            CreateMap<PharmacyToCreateDto, Pharmacy>()
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.Ignore()); // Handled manually via AttachmentService

            // Owner Mapping
            CreateMap<Pharmacy, PharmacyOwnerProfileDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PharmaOwner.ApplicationUser.FullName))
                .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => src.PharmaOwner.ApplicationUser.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime.HasValue ? src.OpenTime.Value.ToString("HH:mm") : null))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime.HasValue ? src.CloseTime.Value.ToString("HH:mm") : null))
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.MapFrom<PictureResolver<Pharmacy, PharmacyOwnerProfileDto>, string>(src => src.LicenseImageUrl));

            // Patient Mapping
            CreateMap<Pharmacy, PharmacyBasicDto>()
                .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.PharmacyName))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating))
                .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src =>
                    src.Is24Hours ||
                    (src.OpenTime.HasValue && src.CloseTime.HasValue &&
                     (src.OpenTime.Value <= src.CloseTime.Value
                         ? (TimeOnly.FromDateTime(DateTime.UtcNow) >= src.OpenTime.Value &&
                            TimeOnly.FromDateTime(DateTime.UtcNow) <= src.CloseTime.Value)
                         : (TimeOnly.FromDateTime(DateTime.UtcNow) >= src.OpenTime.Value ||
                            TimeOnly.FromDateTime(DateTime.UtcNow) <= src.CloseTime.Value)))));

            // Update Mapping (Complete Rewrite)
            CreateMap<PharmacyToUpdateDto, Pharmacy>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.GeneralArea))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PharmaOwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.PharmaOwner, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
