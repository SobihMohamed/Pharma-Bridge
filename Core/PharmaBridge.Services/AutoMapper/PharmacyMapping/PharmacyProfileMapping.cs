using System;
using AutoMapper;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Services.Resolver;

using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.DTOs.PharmacyRating;
namespace PharmaBridge.Services.AutoMapper.PharmacyMapping
{
    public class PharmacyProfileMapping : Profile
    {
        public PharmacyProfileMapping()
        {
            // A. PharmacyToCreateDto -> Pharmacy
            CreateMap<PharmacyToCreateDto, Pharmacy>()
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // B. Pharmacy -> PharmacyOwnerProfileDto
            CreateMap<Pharmacy, PharmacyOwnerProfileDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PharmaOwner.ApplicationUser.FullName))
                .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => src.PharmaOwner.ApplicationUser.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime.HasValue ? src.OpenTime.Value.ToString("HH:mm") : null))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime.HasValue ? src.CloseTime.Value.ToString("HH:mm") : null))
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.MapFrom<PictureResolver<Pharmacy, PharmacyOwnerProfileDto>, string>(src => src.LicenseImageUrl));

            // C. Pharmacy -> PharmacyDto (patient-safe, data-masked)
            CreateMap<Pharmacy, PharmacyDto>()
                .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.PharmacyName))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.TextAddress, opt => opt.MapFrom(src => src.TextAddress))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src =>
                    src.Is24Hours ||
                    (src.OpenTime.HasValue && src.CloseTime.HasValue &&
                     (src.OpenTime.Value <= src.CloseTime.Value
                         ? (TimeOnly.FromDateTime(DateTime.UtcNow) >= src.OpenTime.Value &&
                            TimeOnly.FromDateTime(DateTime.UtcNow) <= src.CloseTime.Value)
                         : (TimeOnly.FromDateTime(DateTime.UtcNow) >= src.OpenTime.Value ||
                            TimeOnly.FromDateTime(DateTime.UtcNow) <= src.CloseTime.Value)))));

            // D. PharmacyToUpdateDto -> Pharmacy
            CreateMap<PharmacyToUpdateDto, Pharmacy>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.GeneralArea))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LicenseImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.LicenseNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PharmaOwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.CompleteOrderCount, opt => opt.Ignore())
                .ForMember(dest => dest.RejectedReasons, opt => opt.Ignore())
                .ForMember(dest => dest.PharmaOwner, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreatePharmacyRatingDto, PharmacyRating>();

            CreateMap<PharmacyRating, PharmacyRatingDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src =>
                    src.PatientProfile != null && src.PatientProfile.ApplicationUser != null
                        ? src.PatientProfile.ApplicationUser.FullName
                        : "Anonymous"));
        }
    }
}