using AutoMapper;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Pharmacy;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.Dto_s.Attachment;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserEnums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public class PharmacyProfileService(IUnitOfWork unitOfWork, IMapper mapper, IAttachementService attachementService) : IPharmacyProfileService
    {
        public async Task<PharmacyOwnerProfileDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, string userId)
        {
            var owner = await ValidatePharmacyRegistrationAsync(userId);

            // ✅ Validate working hours before mapping
            ValidateWorkingHours(createDto.Is24Hours, createDto.OpenTime, createDto.CloseTime);
            //Verify the uniqueness of the license number 
            var isLicenseExist = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>()
             .AnyAsync(p => p.LicenseNumber == createDto.LicenseNumber);
            if (isLicenseExist)
                throw new BadRequestCustomeException("This license number is already registered to another pharmacy.");
            //Verifying the uniqueness of the pharmacy's telephone number
            var isPhoneExist = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>()
            .AnyAsync(p => p.ContactPhone == createDto.ContactPhone);
            if (isPhoneExist)
                throw new BadRequestCustomeException("The pharmacy's phone number is already registered.");

            var pharmacy = mapper.Map<Domain.Models.Pharma_Requests.Pharmacy>(createDto);
            pharmacy.PharmaOwnerId = owner.Id;
            pharmacy.Status = PharmacyStatus.Pending;

            if (createDto.LicenseImage != null)
            {
                var uploadDto = new UploadFileDto
                {
                    File = createDto.LicenseImage,
                    FolderName = $"requests/{userId}",
                    UserId = userId
                };
                pharmacy.LicenseImageUrl = await attachementService.UploadFileAsync(uploadDto);
            }

            await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().AddAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to register pharmacy profile.");

            var spec = new PharmacyWithProfileOwnerSpec(pharmacy.Id);
            var savedPharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);
            return mapper.Map<PharmacyOwnerProfileDto>(savedPharmacy);
        }

        public async Task<PharmacyOwnerProfileDto> GetMyProfileAsync(int pharmacyId, string userId)
        {
            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("Pharmacy not found");

            if (pharmacy.PharmaOwner.ApplicationUserId != userId)
                throw new UnAuthorizedCustomeException("You are not authorized to view this profile.");

            return mapper.Map<PharmacyOwnerProfileDto>(pharmacy);
        }

        public async Task<PharmacyOwnerProfileDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, string userId)
        {
            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("Pharmacy not found.");

            if (pharmacy.PharmaOwner.ApplicationUserId != userId)
                throw new UnAuthorizedCustomeException("You are not authorized to update this profile.");

            if (pharmacy.Status == PharmacyStatus.Pending)
                throw new BadRequestCustomeException("You cannot update the pharmacy profile while it is under review by the administration. Please wait for a response.");

            if (!string.IsNullOrEmpty(updateDto.ContactPhone) && updateDto.ContactPhone != pharmacy.ContactPhone)
            {
                var isPhoneExist = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>()
                    .AnyAsync(p => p.ContactPhone == updateDto.ContactPhone);

                if (isPhoneExist)
                    throw new BadRequestCustomeException("The new phone number is already registered to another pharmacy.");
            }

            mapper.Map(updateDto, pharmacy);

            ValidateWorkingHours(pharmacy.Is24Hours, pharmacy.OpenTime, pharmacy.CloseTime);

            if (pharmacy.Status == PharmacyStatus.Active)
                pharmacy.Status = PharmacyStatus.Pending;

            if (updateDto.LicenseImage != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.LicenseImageUrl))
                    await attachementService.DeleteFileAsync(pharmacy.LicenseImageUrl);

                var uploadDto = new UploadFileDto
                {
                    File = updateDto.LicenseImage,
                    FolderName = $"requests/{userId}",
                    UserId = userId
                };
                pharmacy.LicenseImageUrl = await attachementService.UploadFileAsync(uploadDto);
            }

            // Update the entity in the database
            unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().UpdateAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to update pharmacy profile.");

            return mapper.Map<PharmacyOwnerProfileDto>(pharmacy);
        }

        public async Task<PharmacyDto> GetPharmacyBasicInfoAsync(int pharmacyId)
        {
            var spec = new PharmacyForPatientSpec(pharmacyId);
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("Pharmacy not found or not active yet.");

            return mapper.Map<PharmacyDto>(pharmacy);
        }

        // ✅ New private method — called in both Register and Update
        private void ValidateWorkingHours(bool is24Hours, TimeOnly? openTime, TimeOnly? closeTime)
        {
            if (is24Hours) return;

            if (!openTime.HasValue || !closeTime.HasValue)
                throw new BadRequestCustomeException("Opening and closing times must be specified as long as the pharmacy does not operate 24 hours a day.");

            if (openTime.Value == closeTime.Value)
                throw new BadRequestCustomeException("The opening and closing times cannot be identical.");
        }

        private async Task<PharmaOwner> ValidatePharmacyRegistrationAsync(string userId)
        {
            var user = await unitOfWork.GetRepository<ApplicationUser, string>().GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundCutomeException("User not found.");

            if (user.Role != UserRole.PharmacyOwner)
                throw new UnAuthorizedCustomeException("Only Pharmacy Owners are allowed to register a pharmacy profile.");

            var spec = new PharmaOwnerByAppUserIdSpec(userId);
            var owners = await unitOfWork.GetRepository<PharmaOwner, string>().GetAllWithSpecAsync(spec);
            var owner = owners.FirstOrDefault();

            if (owner == null)
                throw new NotFoundCutomeException("PharmaOwner profile not found. Please complete your owner registration first.");

            if (owner.Status != PharmaOwnerStatus.Approved)
                throw new BadRequestCustomeException("Your owner account is not yet approved by the admin.");
            //To DO 
            if (owner.Pharmacies != null && owner.Pharmacies.Any())
                throw new BadRequestCustomeException("A pharmacy profile already exists for this owner.");

            return owner;
        }
    }
}