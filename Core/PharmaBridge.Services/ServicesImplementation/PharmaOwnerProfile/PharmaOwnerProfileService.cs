using AutoMapper;
using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.PharmaOwnerProfiles;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.PharmaOwners;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PharmaOwner;
using PharmaBridge.Shared.Dto_s.Attachment;
using PharmaBridge.Shared.DTOs.PharmaOwners;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.ServicesImplementation.PharmaOwnerProfile
{
    public class PharmaOwnerProfileService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAttachementService attachmentService) : IPharmaOwnerProfileService
    {

        public async Task<PharmaOwnerDetailsDto> CreateMyProfileAsync(string applicationUserId, PharmaOwnerToCreateDto createDto)
        {
            await EnsureProfileDoesNotExistAsync(applicationUserId);

            var frontIdPath = await UploadProfileImageAsync(createDto.NationalIdFront, applicationUserId);
            var backIdPath = await UploadProfileImageAsync(createDto.NationalIdBack, applicationUserId);
            var syndicatePath = await UploadProfileImageAsync(createDto.SyndicateCardImage, applicationUserId);

            var pharmaOwner = new PharmaOwner
            {
                Id = System.Guid.NewGuid().ToString(),
                ApplicationUserId = applicationUserId,
                NationalId = createDto.NationalId,
                NationalIdFront = frontIdPath,
                NationalIdBack = backIdPath,
                SyndicateCardImage = syndicatePath,
                Status = PharmaOwnerStatus.Pending
            };

            var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
            await ownerRepo.AddAsync(pharmaOwner);

            if (await unitOfWork.SaveChangesAsync() <= 0)
            {
                throw new BadRequestCustomeException("Failed to create PharmaOwner profile");
            }

            var spec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);
            var created = await ownerRepo.GetByIdWithSpecAsync(spec);

            return mapper.Map<PharmaOwnerDetailsDto>(created);
        }

        public async Task<PharmaOwnerDetailsDto> GetMyProfileAsync(string applicationUserId)
        {
            var owner = await GetProfileOrThrowAsync(applicationUserId);
            return mapper.Map<PharmaOwnerDetailsDto>(owner);
        }

        public async Task<PharmaOwnerDetailsDto> UpdateMyProfileAsync(string applicationUserId, PharmaOwnerToUpdateDto updateDto)
        {
            var owner = await GetProfileOrThrowAsync(applicationUserId);

            UpdateBasicInfo(owner, updateDto);

            owner.NationalIdFront = await ReplaceImageAsync(owner.NationalIdFront, updateDto.NationalIdFront, applicationUserId);
            owner.NationalIdBack = await ReplaceImageAsync(owner.NationalIdBack, updateDto.NationalIdBack, applicationUserId);
            owner.SyndicateCardImage = await ReplaceImageAsync(owner.SyndicateCardImage, updateDto.SyndicateCardImage, applicationUserId);

            var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
            ownerRepo.UpdateAsync(owner);

            if (await unitOfWork.SaveChangesAsync() <= 0)
            {
                throw new BadRequestCustomeException("Failed to update PharmaOwner profile");
            }

            return mapper.Map<PharmaOwnerDetailsDto>(owner);
        }

        public async Task<PaginationResponse<PharmaOwnerDto>> GetAllPharmaOwnersAsync(PharmaOwnerQueryParams queryParams)
        {
            NormalizePaginationParams(queryParams);

            var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
            var dataSpec = new PharmaOwnerWithFiltersSpec(queryParams);
            var countSpec = new PharmaOwnerWithFiltersSpec(queryParams.Search);

            var owners = await ownerRepo.GetAllWithSpecAsync(dataSpec);
            var totalItems = await ownerRepo.GetCountAsync(countSpec);

            var data = mapper.Map<IReadOnlyList<PharmaOwnerDto>>(owners);

            return new PaginationResponse<PharmaOwnerDto>(
                queryParams.PageIndex,
                queryParams.PageSize,
                totalItems,
                data
            );
        }

        public async Task<bool> UpdatePharmaOwnerStatusAsync(string pharmaOwnerId, PharmaOwnerStatus status)
        {
            var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
            var owner = await ownerRepo.GetByIdAsync(pharmaOwnerId);

            if (owner == null)
            {
                throw new NotFoundCutomeException($"PharmaOwner with ID {pharmaOwnerId} not found.");
            }

            owner.Status = status;
            ownerRepo.UpdateAsync(owner);

            return await unitOfWork.SaveChangesAsync() > 0;
        }
        private async Task<string> UploadProfileImageAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var uploadDto = new UploadFileDto
            {
                File = file,
                FolderName = "PharmaOwners",
                UserId = userId
            };

            return await attachmentService.UploadFileAsync(uploadDto);
        }

        private async Task<string> ReplaceImageAsync(string oldImagePath, IFormFile newImageFile, string userId)
        {
            if (newImageFile == null || newImageFile.Length == 0) return oldImagePath;

            if (!string.IsNullOrEmpty(oldImagePath))
            {
                await attachmentService.DeleteFileAsync(oldImagePath);
            }

            return await UploadProfileImageAsync(newImageFile, userId);
        }

        private async Task EnsureProfileDoesNotExistAsync(string applicationUserId)
        {
            var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
            var existingSpec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);

            if (await ownerRepo.GetByIdWithSpecAsync(existingSpec) != null)
            {
                throw new BadRequestCustomeException("PharmaOwner profile already exists for this user.");
            }
        }

        private async Task<PharmaOwner> GetProfileOrThrowAsync(string applicationUserId)
        {
            var ownerRepo = unitOfWork.GetRepository<PharmaOwner, string>();
            var spec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);
            var owner = await ownerRepo.GetByIdWithSpecAsync(spec);

            if (owner == null)
            {
                throw new NotFoundCutomeException("PharmaOwner profile not found. Please complete your owner registration first.");
            }

            return owner;
        }

        private void UpdateBasicInfo(PharmaOwner owner, PharmaOwnerToUpdateDto updateDto)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                owner.ApplicationUser.FullName = updateDto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
                owner.ApplicationUser.PhoneNumber = updateDto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateDto.NationalId))
                owner.NationalId = updateDto.NationalId;
        }

        private void NormalizePaginationParams(PharmaOwnerQueryParams queryParams)
        {
            if (queryParams.PageIndex <= 0) queryParams.PageIndex = 1;
            if (queryParams.PageSize <= 0 || queryParams.PageSize > 50) queryParams.PageSize = 10;
        }
    }
}