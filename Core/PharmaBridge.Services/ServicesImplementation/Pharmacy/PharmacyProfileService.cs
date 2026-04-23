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

            var pharmacy = mapper.Map<Domain.Models.Pharma_Requests.Pharmacy>(createDto);
            pharmacy.PharmaOwnerId = owner.Id;
            pharmacy.Status = PharmacyStatus.Pending;

            if (createDto.LicenseImage != null)
            {
                var uploadDto = new UploadFileDto { File = createDto.LicenseImage, FolderName = "Images/Profiles/Licenses" };
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
                throw new NotFoundCutomeException("Pharmacy not found");

            if (pharmacy.PharmaOwner.ApplicationUserId != userId)
                throw new UnAuthorizedCustomeException("You are not authorized to update this profile.");
            
            mapper.Map(updateDto, pharmacy);

            if (pharmacy.Status == PharmacyStatus.Active)
            {
                pharmacy.Status = PharmacyStatus.Pending;
            }

            if (updateDto.LicenseImage != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.LicenseImageUrl))
                {
                    await attachementService.DeleteFileAsync(pharmacy.LicenseImageUrl);
                }

                var uploadDto = new UploadFileDto { File = updateDto.LicenseImage, FolderName = "Images/Profiles/Licenses" };
                pharmacy.LicenseImageUrl = await attachementService.UploadFileAsync(uploadDto);
            }

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

            if (owner.Pharmacies != null && owner.Pharmacies.Any())
                throw new BadRequestCustomeException("A pharmacy profile already exists for this owner.");

            return owner;
        }
    }
}
