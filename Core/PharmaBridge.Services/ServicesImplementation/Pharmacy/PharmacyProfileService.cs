using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Pharmacy;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using PharmaBridge.Shared.Dto_s.Attachment;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserEnums;


namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public class PharmacyProfileService(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager, INotificationService notificationService, IMapper mapper, IAttachementService attachementService) : IPharmacyProfileService
    {
        public async Task<PharmacyOwnerProfileDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, string userId)
        {
            var owner = await ValidatePharmacyRegistrationAsync(userId);

            // Validate working hours before mapping
            ValidateWorkingHours(createDto.Is24Hours, createDto.OpenTime, createDto.CloseTime);

            var licenseSpec = new PharmacyByLicenseNumberSpec(createDto.LicenseNumber);
            var existingLicense = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(licenseSpec);

            if (existingLicense != null)
                throw new BadRequestCustomeException("This license number is already registered to another pharmacy.");

            var phoneSpec = new PharmacyByContactPhoneSpec(createDto.ContactPhone!);
            var existingPhone = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(phoneSpec);

            if (existingPhone != null)
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

            var adminIds = await GetAdminUserIdsAsync();

            if (adminIds != null && adminIds.Any())
            {
                string bodyMessage = $"A new Pharmacy profile (License: {createDto.LicenseNumber}) has been submitted and is pending your approval.";

                foreach (var adminId in adminIds)
                {
                    var message = new NotificationContentDto
                    {
                        UserId = adminId,
                        Subject = "New Pharmacy Registration 🏪",
                        Body = bodyMessage,
                        ReferenceId = null, 
                        Payload = null
                    };

                    await notificationService.SendNotificationAsync(message, Shared.EnumHelper.NotificationEnums.NotificationType.Push);
                }
            }

            var spec = new PharmacyWithProfileOwnerSpec(pharmacy.Id);
            var savedPharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            return mapper.Map<PharmacyOwnerProfileDto>(savedPharmacy);
        }

        public async Task<PharmacyOwnerProfileDto> GetMyProfileAsync(string userId)
        {
            var pharmacyId = await GetPharmacyIdByUserIdAsync(userId);

            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("Pharmacy not found");

            return mapper.Map<PharmacyOwnerProfileDto>(pharmacy);
        }

        public async Task<PharmacyOwnerProfileDto> UpdateMyProfileAsync(PharmacyToUpdateDto updateDto, string userId)
        {
            var pharmacyId = await GetPharmacyIdByUserIdAsync(userId);

            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("Pharmacy not found.");

            if (pharmacy.Status == PharmacyStatus.Pending)
                throw new BadRequestCustomeException("You cannot update the pharmacy profile while it is under review by the administration. Please wait for a response.");

            bool requiresAdminApproval = false;
            string? oldLicenseImageToDelete = null;

            if (!string.IsNullOrEmpty(updateDto.ContactPhone) && updateDto.ContactPhone != pharmacy.ContactPhone)
            {
                var phoneSpec = new PharmacyByContactPhoneSpec(updateDto.ContactPhone!);
                var existingPhone = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(phoneSpec);

                if (existingPhone != null)
                    throw new BadRequestCustomeException("The new phone number is already registered to another pharmacy.");

                requiresAdminApproval = true;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.PharmacyName) && updateDto.PharmacyName != pharmacy.PharmacyName)
                requiresAdminApproval = true;

            if (!string.IsNullOrWhiteSpace(updateDto.GeneralArea) && updateDto.GeneralArea != pharmacy.Area)
                requiresAdminApproval = true;

            if (!string.IsNullOrWhiteSpace(updateDto.TextAddress) && updateDto.TextAddress != pharmacy.TextAddress)
                requiresAdminApproval = true;

            if ((updateDto.Latitude.HasValue && updateDto.Latitude != pharmacy.Latitude) ||
                (updateDto.Longitude.HasValue && updateDto.Longitude != pharmacy.Longitude))
                requiresAdminApproval = true;


            mapper.Map(updateDto, pharmacy);

            ValidateWorkingHours(pharmacy.Is24Hours, pharmacy.OpenTime, pharmacy.CloseTime);

            if (updateDto.LicenseImage != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.LicenseImageUrl))
                    oldLicenseImageToDelete = pharmacy.LicenseImageUrl;

                var uploadDto = new UploadFileDto
                {
                    File = updateDto.LicenseImage,
                    FolderName = $"requests/{userId}",
                    UserId = userId
                };

                pharmacy.LicenseImageUrl = await attachementService.UploadFileAsync(uploadDto);
                requiresAdminApproval = true;
            }

            if (requiresAdminApproval && pharmacy.Status == PharmacyStatus.Active)
            {
                pharmacy.Status = PharmacyStatus.Pending;

                var admins = await userManager.GetUsersInRoleAsync("Admin");
                foreach (var admin in admins)
                {
                    var adminNotification = new NotificationContentDto
                    {
                        UserId = admin.Id,
                        Subject = "Pharmacy Details Update ",
                        Body = $"The pharmacy '{pharmacy.PharmacyName}' has updated critical details (Name/Location/License/Phone) and requires review.",
                        ReferenceId = pharmacy.Id,
                        Payload = null
                    };
                    await notificationService.SendNotificationAsync(adminNotification, Shared.EnumHelper.NotificationEnums.NotificationType.Push);
                }

                var userNotification = new NotificationContentDto
                {
                    UserId = userId,
                    Subject = "PharmaBridge: Pharmacy Update Status",
                    Body = "We have received the updates for your pharmacy. The profile is currently under review by our team.",
                    ReferenceId = pharmacy.Id,
                    Payload = null
                };
                await notificationService.SendNotificationAsync(userNotification, Shared.EnumHelper.NotificationEnums.NotificationType.Push);
            }

            unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().UpdateAsync(pharmacy);

            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to update pharmacy profile.");

            if (!string.IsNullOrEmpty(oldLicenseImageToDelete))
            {
                await attachementService.DeleteFileAsync(oldLicenseImageToDelete);
            }

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

        public async Task<AdminPharmacyDetailsDto> GetPharmacyDetailsForAdminAsync(int pharmacyId)
        {
            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException($"Pharmacy with ID {pharmacyId} not found.");

            return mapper.Map<AdminPharmacyDetailsDto>(pharmacy);
        }

        public async Task<int> GetPharmacyIdByUserIdAsync(string userId)
        {
            var pharmacyRepo = unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();

            var spec = new PharmacyByAppUserIdSpec(userId);

            var pharmacy = await pharmacyRepo.GetByIdWithSpecAsync(spec);

            if (pharmacy == null)
                throw new NotFoundCutomeException("No pharmacy found assigned to this user account.");

            return pharmacy.Id;
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

            if (owner.Pharmacy != null)
                throw new BadRequestCustomeException("A pharmacy profile already exists for this owner.");

            return owner;
        }

        public async Task<PaginationResponse<AdminPharmacyDto>> GetAllPharmaciesAsync(PharmacyQueryParams queryParams)
        {
            if (queryParams.PageIndex <= 0)
                queryParams.PageIndex = 1;

            if (queryParams.PageSize <= 0 || queryParams.PageSize > 50)
                queryParams.PageSize = 10;

            var pharmacyRepo = unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();
            var dataSpec = new PharmacyWithFiltersSpec(queryParams, isCountSpec: false);
            var countSpec = new PharmacyWithFiltersSpec(queryParams, isCountSpec: true);

            var pharmacies = await pharmacyRepo.GetAllWithSpecAsync(dataSpec);
            var totalItems = await pharmacyRepo.GetCountAsync(countSpec);

            var data = mapper.Map<IReadOnlyList<AdminPharmacyDto>>(pharmacies);

            return new PaginationResponse<AdminPharmacyDto>(
                queryParams.PageIndex,
                queryParams.PageSize,
                totalItems,
                data
            );
        }

        public async Task<bool> UpdatePharmacyStatusAsync(int pharmacyId, PharmacyStatus status)
        {
            var pharmacyRepo = unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);

            if (pharmacy == null)
                throw new NotFoundCutomeException($"Pharmacy with ID {pharmacyId} not found.");

            pharmacy.Status = status;
            pharmacyRepo.UpdateAsync(pharmacy);

            var isSaved = await unitOfWork.SaveChangesAsync() > 0;

            if (isSaved)
            {
                var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
                var owner = await ownerRepo.GetByIdAsync(pharmacy.PharmaOwnerId);

                if (owner != null && !string.IsNullOrEmpty(owner.ApplicationUserId))
                {
                    string subject = status == PharmacyStatus.Active
                        ? "Pharmacy Approved! 🏪🎉"
                        : "Pharmacy Status Update ⚠️";

                    string bodyMessage = status == PharmacyStatus.Active
                        ? "Congratulations! Your pharmacy location and license have been approved. You can now receive orders."
                        : $"Your pharmacy profile status has been changed to: {status}. Please check your account.";

                    var message = new NotificationContentDto
                    {
                        UserId = owner.ApplicationUserId, 
                        Subject = subject,
                        Body = bodyMessage,
                        ReferenceId = null,
                        Payload = null
                    };

                    await notificationService.SendNotificationAsync(message, Shared.EnumHelper.NotificationEnums.NotificationType.Push);
                }
            }

            return isSaved;
        }
        private async Task<List<string>> GetAdminUserIdsAsync()
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");

            return admins.Select(admin => admin.Id).ToList();
        }
    }
}