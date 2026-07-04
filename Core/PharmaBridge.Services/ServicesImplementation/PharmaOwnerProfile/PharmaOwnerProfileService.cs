using AutoMapper;
using PharmaBridge.Abstraction.IServices.PharmaOwnerProfiles;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.PharmaOwners;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PharmaOwner;
using PharmaBridge.Shared.DTOs.PharmaOwners;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.PharmaOwnerProfile
{
    public class PharmaOwnerProfileService(IUnitOfWork unitOfWork, IMapper mapper) : IPharmaOwnerProfileService
    {
        public async Task<PharmaOwnerDetailsDto> CreateMyProfileAsync(string applicationUserId, PharmaOwnerToCreateDto createDto)
        {
            var ownerRepo = unitOfWork.GetRepository<Domain.Models.User.PharmaOwner, string>();

            // Check if the user already has a PharmaOwner profile
            var existingSpec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);
            var existingOwner = await ownerRepo.GetByIdWithSpecAsync(existingSpec);

            if (existingOwner != null)
                throw new BadRequestCustomeException("PharmaOwner profile already exists for this user.");

            var pharmaOwner = new Domain.Models.User.PharmaOwner
            {
                Id = System.Guid.NewGuid().ToString(),
                ApplicationUserId = applicationUserId,
                NationalId = createDto.NationalId,
                NationalIdFront = createDto.NationalIdFront,
                NationalIdBack = createDto.NationalIdBack,
                SyndicateCardImage = createDto.SyndicateCardImage
            };

            await ownerRepo.AddAsync(pharmaOwner);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                throw new BadRequestCustomeException("Failed to create PharmaOwner profile");

            // Re-fetch with includes to return full details
            var spec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);
            var created = await ownerRepo.GetByIdWithSpecAsync(spec);

            return mapper.Map<PharmaOwnerDetailsDto>(created);
        }

        public async Task<PharmaOwnerDetailsDto> GetMyProfileAsync(string applicationUserId)
        {
            var spec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);
            var ownerRepo = unitOfWork.GetRepository<Domain.Models.User.PharmaOwner, string>();
            var owner = await ownerRepo.GetByIdWithSpecAsync(spec);

            if (owner == null)
                throw new NotFoundCutomeException("PharmaOwner profile not found. Please complete your owner registration first.");

            return mapper.Map<PharmaOwnerDetailsDto>(owner);
        }

        public async Task<PharmaOwnerDetailsDto> UpdateMyProfileAsync(string applicationUserId, PharmaOwnerToUpdateDto updateDto)
        {
            var ownerRepo = unitOfWork.GetRepository<Domain.Models.User.PharmaOwner, string>();
            var spec = new PharmaOwnerProfileWithDetailsSpec(applicationUserId);
            var owner = await ownerRepo.GetByIdWithSpecAsync(spec);

            if (owner == null)
                throw new NotFoundCutomeException("PharmaOwner profile not found. Please complete your owner registration first.");

            // Update ApplicationUser fields
            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
            {
                owner.ApplicationUser.FullName = updateDto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
            {
                owner.ApplicationUser.PhoneNumber = updateDto.PhoneNumber;
            }

            // Update PharmaOwner document fields
            if (!string.IsNullOrWhiteSpace(updateDto.NationalId))
            {
                owner.NationalId = updateDto.NationalId;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.NationalIdFront))
            {
                owner.NationalIdFront = updateDto.NationalIdFront;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.NationalIdBack))
            {
                owner.NationalIdBack = updateDto.NationalIdBack;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.SyndicateCardImage))
            {
                owner.SyndicateCardImage = updateDto.SyndicateCardImage;
            }

            ownerRepo.UpdateAsync(owner);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                throw new BadRequestCustomeException("Failed to update PharmaOwner profile");

            return mapper.Map<PharmaOwnerDetailsDto>(owner);
        }

        public async Task<PaginationResponse<PharmaOwnerDto>> GetAllPharmaOwnersAsync(PharmaOwnerQueryParams queryParams)
        {
            if (queryParams.PageIndex <= 0)
                queryParams.PageIndex = 1;

            if (queryParams.PageSize <= 0 || queryParams.PageSize > 50)
                queryParams.PageSize = 10;

            var ownerRepo = unitOfWork.GetRepository<Domain.Models.User.PharmaOwner, string>();
            var dataSpec = new PharmaOwnerWithFiltersSpec(queryParams, isCountSpec: false);
            var countSpec = new PharmaOwnerWithFiltersSpec(queryParams, isCountSpec: true);

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

        public async Task<PharmaOwnerDetailsDto> GetPharmaOwnerProfileByIdAsync(string pharmaOwnerId)
        {
            var spec = new PharmaOwnerByIdWithDetailsSpec(pharmaOwnerId);
            var ownerRepo = unitOfWork.GetRepository<Domain.Models.User.PharmaOwner, string>();
            var owner = await ownerRepo.GetByIdWithSpecAsync(spec);

            if (owner == null)
                throw new NotFoundCutomeException($"PharmaOwner profile with ID {pharmaOwnerId} not found.");

            return mapper.Map<PharmaOwnerDetailsDto>(owner);
        }

        public async Task<bool> UpdatePharmaOwnerStatusAsync(string pharmaOwnerId, PharmaBridge.Shared.EnumHelper.PharmaEnums.PharmaOwnerStatus status)
        {
            var ownerRepo = unitOfWork.GetRepository<Domain.Models.User.PharmaOwner, string>();
            var owner = await ownerRepo.GetByIdAsync(pharmaOwnerId);

            if (owner == null)
                throw new NotFoundCutomeException($"PharmaOwner with ID {pharmaOwnerId} not found.");

            owner.Status = status;
            ownerRepo.UpdateAsync(owner);

            var result = await unitOfWork.SaveChangesAsync();
            return result > 0;
        }
    }
}
