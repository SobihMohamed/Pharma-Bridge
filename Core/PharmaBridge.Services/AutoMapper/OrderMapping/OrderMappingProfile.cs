using AutoMapper;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.DTOs.BidItem;
using PharmaBridge.Shared.DTOs.Order;

namespace PharmaBridge.Services.AutoMapper.OrderMapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            MapBidItemToBidItemDto();
            MapOrderToOrderDto();
            MapOrderToOrderDetailsDto();
            MapOrderToAdminOrderDetailsDto();
        }

        private void MapBidItemToBidItemDto()
        {
            CreateMap<BidItem, BidItemDto>()
                //.ForMember(dest => dest.Id,
                //    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ItemName,
                    opt => opt.MapFrom(src => src.ItemName))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.IsAlternative,
                    opt => opt.MapFrom(src => src.IsAlternative))
                .ForMember(dest => dest.AlternativeNote,
                    opt => opt.MapFrom(src => src.AlternativeNote))
                .ForMember(dest => dest.LineTotal,
                    opt => opt.MapFrom(src => src.LineTotal));
        }

        private void MapOrderToOrderDto()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.OrderStatus.ToString()))

                .ForMember(dest => dest.PharmacyName,
                    opt => opt.MapFrom(src => src.Pharmacy.PharmacyName))

                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src => src.PaymentStatus.ToString()))

                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.PatientProfile.ApplicationUser.FullName));

            // Convention handles: Id, Amount, PaymentMethod, PaymentStatus, CreatedAt, PharmacyId
        }
        private void MapOrderToOrderDetailsDto()
        {
            CreateMap<Order, OrderDetailsDto>()
                .ForMember(dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.OrderStatus.ToString()))

                // price info 
                .ForMember(dest => dest.Subtotal,
                    opt => opt.MapFrom(src => src.Bid.Subtotal))
                .ForMember(dest => dest.DeliveryFee,
                    opt => opt.MapFrom(src => src.Bid.DeliveryFee))
                .ForMember(dest => dest.DiscountAmount,
                    opt => opt.MapFrom(src => src.Bid.DiscountAmount))

                // Pharmacy info
                .ForMember(dest => dest.PharmacyName,
                    opt => opt.MapFrom(src => src.Pharmacy.PharmacyName))
                .ForMember(dest => dest.PharmacyPhone,
                    opt => opt.MapFrom(src => src.Pharmacy.ContactPhone))

                // Patient info
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.PatientProfile.ApplicationUser.FullName))
                .ForMember(dest => dest.PatientPhone,
                    opt => opt.MapFrom(src => src.PatientProfile.ApplicationUser.PhoneNumber))

                // Delivery address
                .ForMember(dest => dest.DeliveryAddress,
                    opt => opt.MapFrom(src =>
                        src.PatientAddress == null
                            ? string.Empty : $"{src.PatientAddress.AddressLine} - {src.PatientAddress.City}"))

                // Payment info
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src => src.PaymentStatus.ToString()))

                // Items 
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.Bid.BidItems));

            // Convention handles the rest:
            // Id, Amount, CancelReason, PaymentMethod, PaymentStatus,
            // CreatedAt, CancelledAt, DeliveredAt, CompletedAt,
            // PharmacyId, BidId, PrescriptionRequestId
        }
        private void MapOrderToAdminOrderDetailsDto()
        {
            CreateMap<Order, AdminOrderDetailsDto>()
                .ForMember(dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.OrderStatus.ToString()))

                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src => src.PaymentStatus.ToString()))

                // Price breakdown from Bid
                .ForMember(dest => dest.Subtotal,
                    opt => opt.MapFrom(src => src.Bid.Subtotal))
                .ForMember(dest => dest.DeliveryFee,
                    opt => opt.MapFrom(src => src.Bid.DeliveryFee))
                .ForMember(dest => dest.DiscountAmount,
                    opt => opt.MapFrom(src => src.Bid.DiscountAmount))

                // Pharmacy
                .ForMember(dest => dest.PharmacyName,
                    opt => opt.MapFrom(src => src.Pharmacy.PharmacyName))
                .ForMember(dest => dest.PharmacyPhone,
                    opt => opt.MapFrom(src => src.Pharmacy.ContactPhone))

                // Patient Admin sees PatientProfileId too
                .ForMember(dest => dest.PatientId,
                    opt => opt.MapFrom(src => src.PatientProfileId))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.PatientProfile.ApplicationUser.FullName))
                .ForMember(dest => dest.PatientPhone,
                    opt => opt.MapFrom(src => src.PatientProfile.ApplicationUser.PhoneNumber))

                // Delivery address
                .ForMember(dest => dest.DeliveryAddress,
                    opt => opt.MapFrom(src =>
                        src.PatientAddress == null
                            ? string.Empty : $"{src.PatientAddress.AddressLine}, {src.PatientAddress.City}"))

                // Items
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.Bid.BidItems));
        }
    }
}