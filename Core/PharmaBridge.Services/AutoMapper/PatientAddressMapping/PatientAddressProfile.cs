using AutoMapper;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.DTOs.PatientAddresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.AutoMapper.PatientAddressMapping
{
    public class PatientAddressProfile : Profile
    {
        public PatientAddressProfile() 
        {
            CreateMap<PatientAddress, PatientAddressDto>();

            CreateMap<CreatePatientAddressDto, PatientAddress>();

            CreateMap<UpdatePatientAddressDto, PatientAddress>();
        }
    }
}
