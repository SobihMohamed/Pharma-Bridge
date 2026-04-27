using AutoMapper;
using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.Bidding;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Specifications.Bidding;
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

            var prescriptionRequest = await GetOpenPrescriptionRequestOrThrowAsync(createBidDto.PrescriptionRequestId);
            await EnsureNoDuplicateBidAsync(pharmacyId, createBidDto.PrescriptionRequestId);

            var bid = BuildBid(createBidDto, pharmacyId);

            await PersistBidAndUpdateRequestStatusAsync(bid, prescriptionRequest);

            var savedBid = await GetBidWithDetailsOrThrowAsync(bid.Id);
            return _mapper.Map<BidDetailsDto>(savedBid);
        }

        public async Task<PaginationResponse<BidDto>> GetPharmacyBidsAsync(int pharmacyId, BidQueryParams queryParams)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();

            var dataSpec = new PharmacyBidsSpecification(pharmacyId, queryParams);
            var countSpec = new PharmacyBidsCountSpecification(pharmacyId, queryParams);

            var bids = await bidRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await bidRepo.GetCountAsync(countSpec);

            return BuildPaginatedResult<BidDto>(bids, totalCount, queryParams);
        }

        public async Task<PaginationResponse<BidDto>> GetAllPlatformBidsAsync(BidQueryParams queryParams)
        {
            var bidRepo = _unitOfWork.GetRepository<Bid, int>();

            var dataSpec = new AdminBidsSpecification(queryParams);
            var countSpec = new AdminBidsCountSpecification(queryParams);

            var bids = await bidRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await bidRepo.GetCountAsync(countSpec);

            return BuildPaginatedResult<BidDto>(bids, totalCount, queryParams);
        }

        public async Task<AdminBidDetailsDto> GetAdminBidDetailsAsync(int bidId)
        {
            var bid = await GetBidWithDetailsOrThrowAsync(bidId);
            return _mapper.Map<AdminBidDetailsDto>(bid);
        }

        // Private Helpers
        private async Task EnsurePharmacyOwnershipAsync(int pharmacyId)
        {
            var currentUserId = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();
                //throw new UnAuthorizedCustomeException("Unable to identify the authenticated user.");

            var pharmacyRepo = _unitOfWork.GetRepository<Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);


            if (pharmacy is null || pharmacy.IsDeleted)
                throw new NotFoundCutomeException($"Pharmacy with ID {pharmacyId} was not found.");

            if (pharmacy.PharmaOwnerId != currentUserId)
                throw new UnAuthorizedCustomeException();
                //throw new UnAuthorizedCustomeException("You are not authorized to submit a bid for this pharmacy.");

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
    }
}