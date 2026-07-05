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
            string actualProfileId = await GetActualProfileIdAsync(patientId);
            var addresses = await FetchAddressesForPatientAsync(actualProfileId);
            return mapper.Map<IReadOnlyList<PatientAddressDto>>(addresses);

        }

        public async Task<PatientAddressDto> AddAddressAsync(string patientId, CreatePatientAddressDto createDto)
        {
            string actualProfileId = await GetActualProfileIdAsync(patientId);

            var address = mapper.Map<PharmaBridge.Domain.Models.User.PatientAddress>(createDto);
            address.PatientProfileId = actualProfileId;

            await HandleAddressDefaultStateAsync(actualProfileId, address);

            await unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().AddAsync(address);
            await CommitChangesAsync("Failed to add new patient address.");

            return mapper.Map<PatientAddressDto>(address);
        }

        public async Task<PatientAddressDto> UpdateAddressAsync(int addressId, string patientId, UpdatePatientAddressDto updateDto)
        {

            string actualProfileId = await GetActualProfileIdAsync(patientId);
            var existingAddress = await FetchAddressOrThrowAsync(addressId, actualProfileId); 

            mapper.Map(updateDto, existingAddress);

            unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().UpdateAsync(existingAddress);
            await CommitChangesAsync("Failed to update patient address.");

            return mapper.Map<PatientAddressDto>(existingAddress);
        }

        public async Task<bool> DeleteAddressAsync(int addressId, string patientId)
        {
            string actualProfileId = await GetActualProfileIdAsync(patientId);
            var address = await FetchAddressOrThrowAsync(addressId, actualProfileId);

            unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().DeleteAsync(address);
            await CommitChangesAsync("Failed to delete patient address.");

            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int addressId, string patientId)
        {
            string actualProfileId = await GetActualProfileIdAsync(patientId);
            var newDefaultAddress = await FetchAddressOrThrowAsync(addressId, actualProfileId);
            await ApplySetDefaultBusinessRuleAsync(actualProfileId, newDefaultAddress);

            await CommitChangesAsync("Failed to set default patient address.");
            return true;
        }


        #region Helper Methods

        private async Task<IReadOnlyList<PharmaBridge.Domain.Models.User.PatientAddress>> FetchAddressesForPatientAsync(string profileId)
        {
            var spec = new PatientAddressesByPatientIdSpec(profileId);
            
            return await unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().GetAllWithSpecAsync(spec);
        }

        private async Task<PharmaBridge.Domain.Models.User.PatientAddress> FetchAddressOrThrowAsync(int addressId, string profileId)
        {
            var spec = new PatientAddressByIdAndPatientIdSpec(addressId, profileId);
            var address = await unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>().GetByIdWithSpecAsync(spec);
            if (address == null)
                throw new NotFoundCutomeException($"Address with Id '{addressId}' was not found or access is denied.");
            return address;
        }

        private async Task HandleAddressDefaultStateAsync(string profileId, PharmaBridge.Domain.Models.User.PatientAddress addressToSave)
        {
            var addressRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>();
            var existingAddresses = await FetchAddressesForPatientAsync(profileId);

            if (existingAddresses.Count == 0)
            {
                addressToSave.IsDefault = true;
            }
            else if (addressToSave.IsDefault)
            {
                foreach (var oldAddress in existingAddresses)
                {
                    if (oldAddress.IsDefault && oldAddress.Id != addressToSave.Id)
                    {
                        oldAddress.IsDefault = false;
                        addressRepo.UpdateAsync(oldAddress);
                    }
                }
            }
        }

        private async Task ApplySetDefaultBusinessRuleAsync(string profileId, PharmaBridge.Domain.Models.User.PatientAddress newDefaultAddress)
        {
            var addressRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.User.PatientAddress, int>();
            var allAddresses = await FetchAddressesForPatientAsync(profileId);
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

        private async Task<string> GetActualProfileIdAsync(string applicationUserId)
        {
            var spec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var profile = await unitOfWork.GetRepository<PatientProfile, string>().GetAllWithSpecAsync(spec);

            var actualProfile = profile.FirstOrDefault();
            if (actualProfile == null)
                throw new NotFoundCutomeException("Patient Profile not found for this user.");

            return actualProfile.Id;
        }

        #endregion
    }
}

