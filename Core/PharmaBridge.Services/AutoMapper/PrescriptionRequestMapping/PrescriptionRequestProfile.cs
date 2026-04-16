using AutoMapper;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Resolver;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.AutoMapper.PrescriptionRequestMapping
{
    public class PrescriptionRequestProfile : Profile
    {
        public PrescriptionRequestProfile()
        {
            // 1 - Mapping From Creation Dto to Entity (Input)
            CreateMap<CreatePrescriptionRequestDto, PrescriptionRequestEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Status is set to Pending by default in the entity
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore()) // system 
                .ForMember(dest => dest.Order, opt => opt.Ignore()) // Order is null when creating a new request
                .ForMember(dest => dest.PrescriptionRequestHistorys, opt => opt.Ignore()) // No history when creating a new request
                .ForMember(dest => dest.Bids, opt => opt.Ignore()) // No bids when creating a new request
                .ForMember(dest => dest.PatientProfileId, opt => opt.Ignore()); // PatientProfileId is set separately

            // 2 - Mapping From Entity to Dto (Output)
            CreateMap<PrescriptionRequestEntity, PrescriptionRequestDto>()
                
                .ForMember(dest => dest.ImageUrl, opt => 
                opt.MapFrom<PictureResolver<PrescriptionRequestEntity, PrescriptionRequestDto>, string>(src => src.ImageUrl!))
                
                .ForMember(dest => dest.Status , opt => opt.MapFrom(src => src.Status.ToString()))

                .ForMember(dest => dest.BidsCount, opt => opt.MapFrom(src => src.Bids != null ? src.Bids.Count : 0))

                .ForMember(dest => dest.DeliveryArea , opt => opt.MapFrom(src => 
                    src.DeliveryAddress != null ? $"{src.DeliveryAddress.City} - {src.DeliveryAddress.AddressLine}" : "Unknown"));
        }
    }
}
