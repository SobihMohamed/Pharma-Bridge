using AutoMapper;
using PharmaBridge.Abstraction.IServices.PatientAddresses;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Services.Specifications.PatientAddressSpec;
using PharmaBridge.Shared.DTOs.PatientAddresses;
using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.ServicesImplementation.PatientAddress
{
    public class PatientAddressService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientAddressService
    {
        public async Task<IReadOnlyList<PatientAddressDto>> GetPatientAddressesAsync(string patientId)
        {
            var addresses = await FetchAddressesForPatientAsync(patientId);
            return mapper.Map<IReadOnlyList<PatientAddressDto>>(addresses);
        }

        public async Task<PatientAddressDto> AddAddressAsync(string patientId, CreatePatientAddressDto createDto)
        {
            var address = mapper.Map<PharmaBridge.Domain.Models.User.PatientAddress>(createDto);
            address.PatientProfileId = patientId;
            await HandleFirstAddressDefaultRuleAsync(patientId, address);

            await unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().AddAsync(address);
            await CommitChangesAsync("Failed to add new patient address.");

            return mapper.Map<PatientAddressDto>(address);
        }

        public async Task<PatientAddressDto> UpdateAddressAsync(int addressId, string patientId, UpdatePatientAddressDto updateDto)
        {
            var existingAddress = await FetchAddressOrThrowAsync(addressId, patientId);

            mapper.Map(updateDto, existingAddress);

            unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().UpdateAsync(existingAddress);
            await CommitChangesAsync("Failed to update patient address.");

            return mapper.Map<PatientAddressDto>(existingAddress);
        }

        public async Task<bool> DeleteAddressAsync(int addressId, string patientId)
        {
            var address = await FetchAddressOrThrowAsync(addressId, patientId);

            unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().DeleteAsync(address);
            await CommitChangesAsync("Failed to delete patient address.");

            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int addressId, string patientId)
        {
            var newDefaultAddress = await FetchAddressOrThrowAsync(addressId, patientId);
            await ApplySetDefaultBusinessRuleAsync(patientId, newDefaultAddress);
            await CommitChangesAsync("Failed to set default patient address.");
            return true;
        }


        #region Helper Methods

        private async Task<IReadOnlyList<PharmaBridge.Domain.Models.User.PatientAddress>> FetchAddressesForPatientAsync(string patientId)
        {
            var spec = new PatientAddressesByPatientIdSpec(patientId);
            
            return await unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().GetAllWithSpecAsync(spec);
        }

        private async Task<PharmaBridge.Domain.Models.User.PatientAddress> FetchAddressOrThrowAsync(int addressId, string patientId)
        {
            var spec = new PatientAddressByIdAndPatientIdSpec(addressId, patientId);
            var address = await unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().GetByIdWithSpecAsync(spec);
            if (address == null)
                throw new NotFoundCutomeException($"Address with Id '{addressId}' was not found or access is denied.");
            return address;
        }

        private async Task HandleFirstAddressDefaultRuleAsync(string patientId, PharmaBridge.Domain.Models.User.PatientAddress newAddress)
        {
            var currentAddresses = await FetchAddressesForPatientAsync(patientId);

            if (currentAddresses.Count == 0)
            {
                newAddress.IsDefault = true;
            }
        }

        private async Task ApplySetDefaultBusinessRuleAsync(string patientId, PharmaBridge.Domain.Models.User.PatientAddress newDefaultAddress)
        {
            var addressRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>();
            var allAddresses = await FetchAddressesForPatientAsync(patientId);
            foreach (var address in allAddresses)
            {
                if (address.IsDefault)
                {
                    address.IsDefault = false;
                    addressRepo.UpdateAsync(address);
                }
            }
            newDefaultAddress.IsDefault = true;
            addressRepo.UpdateAsync(newDefaultAddress);
        }

        private async Task CommitChangesAsync(string errorMessage)
        {
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0)
                throw new BadRequestCustomeException(errorMessage);
        }

        #endregion
    }
}

