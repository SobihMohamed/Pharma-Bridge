using AutoMapper;
using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.Bidding;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Bidding;
using PharmaBridge.Services.Specifications.PharmaOwners;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Bid;
using PharmaBridge.Shared.DTOs.Bid;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System.Security.Claims;

namespace PharmaBridge.Services.Bidding
{
    public class BidService : IBidService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BidService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        // Public Methods
        public async Task<BidDetailsDto> CreateBidAsync(CreateBidDto createBidDto, int pharmacyId)
        {
            await EnsurePharmacyOwnershipAsync(pharmacyId);

            if (createBidDto.BidItems.Count == 0)
                throw new BadRequestCustomeException("BidItems cannot be empty.");

            var prescriptionRequest = await GetOpenPrescriptionRequestOrThrowAsync(createBidDto.PrescriptionRequestId);
            await EnsureNoDuplicateBidAsync(pharmacyId, createBidDto.PrescriptionRequestId);

            var bid = BuildBid(createBidDto, pharmacyId);

            await PersistBidAndUpdateRequestStatusAsync(bid, prescriptionRequest);

            var savedBid = await GetBidWithDetailsOrThrowAsync(bid.Id);
            return _mapper.Map<BidDetailsDto>(savedBid);
        }

        public async Task<PaginationResponse<BidDto>> GetPharmacyBidsAsync(int pharmacyId, BidQueryParams queryParams)
        {
            //await EnsurePharmacyOwnershipAsync(pharmacyId);

            var bidRepo = _unitOfWork.GetRepository<Bid, int>();

            var dataSpec = new PharmacyBidsSpecification(pharmacyId, queryParams);
            var countSpec = new PharmacyBidsCountSpecification(pharmacyId, queryParams);

            var bids = await bidRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await bidRepo.GetCountAsync(countSpec);

            return BuildPaginatedResult<BidDto>(bids, totalCount, queryParams);
        }

        public async Task<PaginationResponse<BidDto>> GetAllPlatformBidsAsync(BidQueryParams queryParams)
        {
            //EnsureAdmin();
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();

            var dataSpec = new AdminBidsSpecification(queryParams);
            var countSpec = new AdminBidsCountSpecification(queryParams);

            var bids = await bidRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await bidRepo.GetCountAsync(countSpec);

            return BuildPaginatedResult<BidDto>(bids, totalCount, queryParams);
        }

        public async Task<AdminBidDetailsDto> GetAdminBidDetailsAsync(int bidId)
        {
            //EnsureAdmin();
            var bid = await GetBidWithDetailsOrThrowAsync(bidId);
            return _mapper.Map<AdminBidDetailsDto>(bid);
        }


        // Phase 2 - public
        public async Task<BidDetailsDto> UpdateBidAsync(UpdateBidDto updateBidDto, int pharmacyId)
        {
            await EnsurePharmacyOwnershipAsync(pharmacyId);

            var bid = await GetBidWithDetailsOrThrowAsync(updateBidDto.Id);

            EnsureBidBelongsToPharmacy(bid, pharmacyId);
            EnsureBidIsPending(bid);

            ApplyBidUpdates(updateBidDto, bid);

            bid.Pharmacy = null;
            bid.PrescriptionRequest = null;
            
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();
            bidRepo.UpdateAsync(bid);

            var bidItemRepo = _unitOfWork.GetRepository<BidItem, int>();
            foreach (var item in bid.BidItems)
            {
                bidItemRepo.UpdateAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedBid = await GetBidWithDetailsOrThrowAsync(bid.Id);
            return _mapper.Map<BidDetailsDto>(updatedBid);
        }

        public async Task<PaginationResponse<BidDto>> GetBidsForRequestAsync(int requestId, string patientId, BidQueryParams queryParams)
        {
            await EnsurePatientOwnsRequestAsync(requestId);

            var bidRepo = _unitOfWork.GetRepository<Bid, int>();

            var dataSpec = new BidsByRequestSpecification(requestId, queryParams);
            var countSpec = new BidsByRequestCountSpecification(requestId, queryParams);

            var bids = await bidRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await bidRepo.GetCountAsync(countSpec);

            return BuildPaginatedResult<BidDto>(bids, totalCount, queryParams);
        }

        public async Task<bool> RespondToBidAsync(int bidId, string patientId, BidStatus status)
        {
            if (status != BidStatus.Accepted && status != BidStatus.Rejected)
                throw new BadRequestCustomeException("Status must be either Accepted or Rejected.");

            var bid = await GetBidWithDetailsOrThrowAsync(bidId);

            await EnsurePatientOwnsRequestOfTheBidAsync(bid);
            EnsureBidIsPending(bid);

            if (status == BidStatus.Accepted)
                await ExecuteAcceptBidTransactionAsync(bid);
            else
                await ExecuteRejectBidTransactionAsync(bid);

            return true;
        }

        private async Task ExecuteRejectBidTransactionAsync(Bid bid)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();

            bid.Status = BidStatus.Rejected;
            bid.RespondedAt = DateTime.UtcNow;
            bidRepo.UpdateAsync(bid);

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<BidDetailsDto> GetBidDetailsAsync(int bidId)
        {
            var bid = await GetBidWithDetailsOrThrowAsync(bidId);
            await EnsureCurrentUserCanViewBidAsync(bid);
            return _mapper.Map<BidDetailsDto>(bid);
        }

        private async Task EnsurePharmacyOwnershipAsync(int pharmacyId)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();

            var ownerRepo = _unitOfWork.GetRepository<PharmaOwner, string>();
            var ownerSpec = new PharmaOwnerByAppUserIdSpecification(currentUserId);

            var currentOwner = await ownerRepo.GetByIdWithSpecAsync(ownerSpec);

            if (currentOwner == null)
                throw new UnAuthorizedCustomeException();

            var pharmacyRepo = _unitOfWork.GetRepository<Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);

            if (pharmacy is null || pharmacy.IsDeleted)
                throw new NotFoundCutomeException($"Pharmacy with ID {pharmacyId} was not found.");

            if (pharmacy.PharmaOwnerId != currentOwner.Id)
                throw new UnAuthorizedCustomeException();

            if (pharmacy.Status == PharmacyStatus.Pending)
                throw new BadRequestCustomeException("Your pharmacy account is not approved yet. You cannot submit bids.");

            if (pharmacy.Status == PharmacyStatus.Blocked)
                throw new BadRequestCustomeException("Your pharmacy account is Blocked. You cannot submit bids.");
        }

        private async Task<PrescriptionRequestEntity> GetOpenPrescriptionRequestOrThrowAsync(int prescriptionRequestId)
        {
            var repo = _unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var request = await repo.GetByIdAsync(prescriptionRequestId);


            if (request is null || request.IsDeleted)
                throw new NotFoundCutomeException($"Prescription request with ID {prescriptionRequestId} was not found.");

            bool isOpen = request.Status == PrescriptionStatus.Pending
                        || request.Status == PrescriptionStatus.HasBids;

            if (!isOpen)
                throw new BadRequestCustomeException("Bids can only be submitted for open prescription requests (Pending or HasBids).");

            return request;
        }

        private async Task EnsureNoDuplicateBidAsync(int pharmacyId, int prescriptionRequestId)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();
            var duplicateSpec = new DuplicateBidCheckSpecification(pharmacyId, prescriptionRequestId);
            var existing = await bidRepo.GetByIdWithSpecAsync(duplicateSpec);

            if (existing is not null)
                throw new BadRequestCustomeException("This pharmacy has already submitted a bid for this prescription request.");
        }

        private Bid BuildBid(CreateBidDto dto, int pharmacyId)
        {
            var bid = _mapper.Map<Bid>(dto);
            bid.PharmacyId = pharmacyId;
            bid.TotalPrice = dto.Subtotal - dto.DiscountAmount + dto.DeliveryFee;

            foreach (var item in bid.BidItems)
                item.LineTotal = item.UnitPrice * item.Quantity;

            return bid;
        }

        private async Task PersistBidAndUpdateRequestStatusAsync(
            Bid bid,
            PrescriptionRequestEntity prescriptionRequest)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();
            var prescriptionRepo = _unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            await bidRepo.AddAsync(bid);

            prescriptionRequest.Status = PrescriptionStatus.HasBids;
            prescriptionRepo.UpdateAsync(prescriptionRequest);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<Bid> GetBidWithDetailsOrThrowAsync(int bidId)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();
            var spec = new BidWithDetailsSpecification(bidId);
            var bid = await bidRepo.GetByIdWithSpecAsync(spec);

            if (bid is null)
                throw new NotFoundCutomeException($"Bid with ID {bidId} was not found.");

            return bid;
        }

        private PaginationResponse<TDto> BuildPaginatedResult<TDto>(
            IReadOnlyList<Bid> bids,
            int totalCount,
            BidQueryParams queryParams)
        {
            var data = _mapper.Map<IReadOnlyList<TDto>>(bids);
            return new PaginationResponse<TDto>(queryParams.PageIndex, queryParams.PageSize, totalCount, data);
        }

        // Phase 2 - private

        private async Task EnsurePatientOwnsRequestAsync(int requestId)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();


            var patientRepo = _unitOfWork.GetRepository<PatientProfile, string>();
            var patientSpec = new PatientProfileByAppUserIdSpec(currentUserId);

            var currentPatient = await patientRepo.GetByIdWithSpecAsync(patientSpec);

            if (currentPatient == null)
                throw new UnAuthorizedCustomeException();


            var requestRepo = _unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var request = await requestRepo.GetByIdAsync(requestId);

            if (request is null || request.IsDeleted)
                throw new NotFoundCutomeException($"Prescription request with ID {requestId} was not found.");


            if (request.PatientProfileId != currentPatient.Id)
                throw new UnAuthorizedCustomeException();
        }

        private async Task EnsurePatientOwnsRequestOfTheBidAsync(Bid bid)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();

            var patientRepo = _unitOfWork.GetRepository<PatientProfile, string>();
            var patientSpec = new PatientProfileByAppUserIdSpec(currentUserId);
            var currentPatient = await patientRepo.GetByIdWithSpecAsync(patientSpec);

            if (currentPatient == null)
                throw new UnAuthorizedCustomeException();

            if (bid.PrescriptionRequest.PatientProfileId != currentPatient.Id)
                throw new UnAuthorizedCustomeException();
        }

        private async Task EnsureCurrentUserCanViewBidAsync(Bid bid)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();


            var ownerRepo = _unitOfWork.GetRepository<PharmaOwner, string>();
            var ownerSpec = new PharmaOwnerByAppUserIdSpecification(currentUserId);
            var currentOwner = await ownerRepo.GetByIdWithSpecAsync(ownerSpec);

            bool isPharmacyOwner = currentOwner != null && bid.Pharmacy.PharmaOwnerId == currentOwner.Id;


            var patientRepo = _unitOfWork.GetRepository<PatientProfile, string>();
            var patientSpec = new PatientProfileByAppUserIdSpec(currentUserId);
            var currentPatient = await patientRepo.GetByIdWithSpecAsync(patientSpec);

            bool isPatient = currentPatient != null && bid.PrescriptionRequest.PatientProfileId == currentPatient.Id;

           
            if (!isPharmacyOwner && !isPatient)
                throw new UnAuthorizedCustomeException();
        }

        private void EnsureBidBelongsToPharmacy(Bid bid, int pharmacyId)
        {
            if (bid.PharmacyId != pharmacyId)
                throw new UnAuthorizedCustomeException();
        }

        private void EnsureBidIsPending(Bid bid)
        {
            if (bid.Status != BidStatus.Pending)
                throw new BadRequestCustomeException(
                    $"This action can only be performed on a Pending bid. Current status: {bid.Status}.");
        }

        private async Task ExecuteAcceptBidTransactionAsync(Bid bid)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();
            var prescriptionRepo = _unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            bid.Status = BidStatus.Accepted;
            bid.RespondedAt = DateTime.UtcNow;
            bidRepo.UpdateAsync(bid);

            await RejectOtherBidsAsync(bid.PrescriptionRequestId, bid.Id, bidRepo);

            bid.PrescriptionRequest.Status = PrescriptionStatus.Closed;
            prescriptionRepo.UpdateAsync(bid.PrescriptionRequest);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RejectOtherBidsAsync(int prescriptionRequestId, int acceptedBidId,
            IGenericRepo<Bid, int> bidRepo)
        {
            var otherBidsSpec = new OtherBidsByRequestSpecification(prescriptionRequestId, acceptedBidId);
            var otherBids = await bidRepo.GetAllWithSpecAsync(otherBidsSpec);

            foreach (var otherBid in otherBids)
            {
                otherBid.Status = BidStatus.Rejected;
                otherBid.RespondedAt = DateTime.UtcNow;
                bidRepo.UpdateAsync(otherBid);
            }
        }

        private void ApplyBidUpdates(UpdateBidDto dto, Bid bid)
        {
            _mapper.Map(dto, bid);
            bid.TotalPrice = dto.Subtotal - dto.DiscountAmount + dto.DeliveryFee;

            if (dto.BidItems is null) return;

            foreach (var itemDto in dto.BidItems)
            {
                var existingItem = bid.BidItems.FirstOrDefault(i => i.Id == itemDto.Id);

                if (existingItem is null)
                    throw new NotFoundCutomeException(
                        $"BidItem with ID {itemDto.Id} was not found in this bid.");

                _mapper.Map(itemDto, existingItem);
                existingItem.LineTotal = itemDto.UnitPrice * itemDto.Quantity;
            }
        }

        

    }
}