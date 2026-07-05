using AutoMapper;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.DTOs.Bid;
using PharmaBridge.Shared.DTOs.BidItem;

namespace PharmaBridge.Services.AutoMapper.BidMapping
{
    public class BidProfile : Profile
    {
        public BidProfile()
        {
            // Create Bid
            CreateMap<CreateBidDto, Bid>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PlatformFee, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RespondedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Pharmacy, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.PrescriptionRequest, opt => opt.Ignore())
                .ForMember(dest => dest.BidItems, opt => opt.MapFrom(src => src.BidItems));

            CreateMap<CreateBidItemDto, BidItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BidId, opt => opt.Ignore())
                .ForMember(dest => dest.Bid, opt => opt.Ignore())
                .ForMember(dest => dest.LineTotal, opt => opt.Ignore());

            // Read
            CreateMap<Bid, BidDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.Pharmacy.PharmacyName))
                .ForMember(dest => dest.PharmacyRating, opt => opt.MapFrom(src => src.Pharmacy.AverageRating))
                .ForMember(dest => dest.BidItems, opt => opt.MapFrom(src => src.BidItems));

            CreateMap<Bid, BidDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.Pharmacy.PharmacyName))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order != null ? (int?)src.Order.Id : null))
                .ForMember(dest => dest.BidItems, opt => opt.MapFrom(src => src.BidItems));

            CreateMap<Bid, AdminBidDetailsDto>()
                .IncludeBase<Bid, BidDetailsDto>();

            CreateMap<BidItem, BidItemDto>();


            // Update
            CreateMap<UpdateBidDto, Bid>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RespondedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PlatformFee, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()) // recalculated in service
                .ForMember(dest => dest.PharmacyId, opt => opt.Ignore())
                .ForMember(dest => dest.PrescriptionRequestId, opt => opt.Ignore())

                .ForMember(dest => dest.Pharmacy, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.PrescriptionRequest, opt => opt.Ignore())
                .ForMember(dest => dest.BidItems, opt => opt.Ignore()); // handled per-item in service

            CreateMap<UpdateBidItemDto, BidItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.BidId, opt => opt.Ignore())
                .ForMember(dest => dest.Bid, opt => opt.Ignore())
                .ForMember(dest => dest.LineTotal, opt => opt.Ignore()); // recalculated in service

        }
    }
}