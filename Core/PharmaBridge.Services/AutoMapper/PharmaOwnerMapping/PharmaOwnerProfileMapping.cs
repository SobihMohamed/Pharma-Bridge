using AutoMapper;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.DTOs.PharmaOwners;
using PharmaBridge.Services.Resolver;

namespace PharmaBridge.Services.AutoMapper.PharmaOwnerMapping
{
    public class PharmaOwnerProfileMapping : Profile
    {
        public PharmaOwnerProfileMapping()
        {
            // PharmaOwner → PharmaOwnerDto
            CreateMap<PharmaOwner, PharmaOwnerDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.ApplicationUser.FullName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.ApplicationUser.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.ApplicationUser.PhoneNumber))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            // PharmaOwner → PharmaOwnerDetailsDto
            CreateMap<PharmaOwner, PharmaOwnerDetailsDto>()
                .IncludeBase<PharmaOwner, PharmaOwnerDto>()
                .ForMember(dest => dest.NationalIdFront, opt => opt.MapFrom<PictureResolver<PharmaOwner, PharmaOwnerDetailsDto>, string>(src => src.NationalIdFront))
                .ForMember(dest => dest.NationalIdBack, opt => opt.MapFrom<PictureResolver<PharmaOwner, PharmaOwnerDetailsDto>, string>(src => src.NationalIdBack))
                .ForMember(dest => dest.SyndicateCardImage, opt => opt.MapFrom<PictureResolver<PharmaOwner, PharmaOwnerDetailsDto>, string>(src => src.SyndicateCardImage));
        }
    }
}
