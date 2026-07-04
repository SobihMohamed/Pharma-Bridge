using AutoMapper;
using MediatR;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Events;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Exceptions.NotFoundHandeler.Request;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.Dto_s.Attachment;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.DTOs.PharmaRequests.AdminReq;

namespace PharmaBridge.Services.ServicesImplementation.PrescriptionRequest
{
    public partial class PrescriptionRequestService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAttachementService attachementService,
        IMediator mediator) : IPrescriptionRequestService
    {
        #region Patient Services
        public async Task<PrescriptionRequestDto> CreateRequestAsync(CreatePrescriptionRequestDto createDto, Guid userIdFromToken)
        {
            var applicationUserId = userIdFromToken.ToString();
            var actualPatientProfileId = await GetValidPatientProfileIdAsync(applicationUserId);

            ValidateRequestInput(createDto);
            var patientAddress = await GetValidAddressAsync(createDto.DeliveryAddressId, actualPatientProfileId);

            string? uploadedImageUrl = null;
            if (createDto.ImageUrl != null && createDto.ImageUrl.Length > 0)
            {
                var uploadFolderDto = new UploadFileDto()
                {
                    File = createDto.ImageUrl,
                    FolderName = $"Prescriptions/{actualPatientProfileId}"
                };
                uploadedImageUrl = await attachementService.UploadFileAsync(uploadFolderDto);
            }

            var request = BuidPrescriptionRequestEntity(createDto, actualPatientProfileId, applicationUserId);
            request.ImageUrl = uploadedImageUrl;

            await unitOfWork.GetRepository<PrescriptionRequestEntity, int>().AddAsync(request);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0) throw new BadRequestCustomeException("Failed to create prescription request");
            // Publish event for new prescription request creation
            await mediator.Publish(new PrescriptionRequestCreatedEvent(request.Id));

            var requestDto = mapper.Map<PrescriptionRequestDto>(request);
            requestDto.DeliveryArea = $"{patientAddress.City} - {patientAddress.AddressLine}";
            return requestDto;
        }

        public async Task<PaginationResponse<PrescriptionRequestDto>> GetPatientRequestsAsync(Guid userIdFromToken, PrescriptionRequestQueryParams queryParams)
        {
            var applicationUserId = userIdFromToken.ToString();
            var actualPatientProfileId = await GetValidPatientProfileIdAsync(applicationUserId);
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            var dataSpec = new PatientRequestWithAddressandBidsSpec(actualPatientProfileId, queryParams);
            var countSpec = new PatientRequestWithAddressandBidsCountSpec(actualPatientProfileId, queryParams);

            var requests = await requestRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await requestRepo.GetCountAsync(countSpec);

            var mappedRequests = mapper.Map<IReadOnlyList<PrescriptionRequestDto>>(requests);

            return new PaginationResponse<PrescriptionRequestDto>(
                index: queryParams.PageIndex,
                size: queryParams.PageSize,
                total: totalCount,
                data: mappedRequests
            );
        }

        public async Task<PrescriptionRequestDetailsDto> GetPatientRequestDetailsAsync(int requestId, Guid userIdFromToken)
        {
            var applicationUserId = userIdFromToken.ToString();
            var actualPatientProfileId = await GetValidPatientProfileIdAsync(applicationUserId);

            var spec = new PatientRequestDetailsWithIncludesSpec(requestId, actualPatientProfileId);
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var requestDetails = await requestRepo.GetByIdWithSpecAsync(spec);

            if (requestDetails == null)
                throw new RequestNotFoundException("Prescription Request Not Found");

            return mapper.Map<PrescriptionRequestDetailsDto>(requestDetails);
        }

        public async Task<bool> CancelRequestAsync(int requestId, Guid userIdFromToken)
        {
            var applicationUserId = userIdFromToken.ToString();
            var patientProfileId = await GetValidPatientProfileIdAsync(applicationUserId);

            var request = await GetValidRequestForCancellationAsync(requestId, patientProfileId);
            ProcessRequestCancellation(request, applicationUserId);

            var result = await unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        #endregion

        #region Pharmacy Services
        public async Task<PaginationResponse<PharmacyNearbyRequestDto>> GetNearbyRequestsAsync(int pharmacyId, PrescriptionRequestQueryParams queryParams)
        {
            // get pharmacy
            var pharmacy = await GetValidPharmacyAsync(pharmacyId);

            // calculate the radius
            double radiusKm = queryParams.RadiusInKm ?? 5.0;
            decimal radiusInDegrees = (decimal)(radiusKm / 111.0);

            // build the specification
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var dataSpec = new NearbyRequestsSpec(pharmacy, queryParams, radiusInDegrees);
            var countSpec = new NearbyRequestsCountSpec(pharmacy, queryParams, radiusInDegrees);

            // execute
            var nearbyRequests = await requestRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await requestRepo.GetCountAsync(countSpec);

            // map results
            var mappedRequests = mapper.Map<IReadOnlyList<PharmacyNearbyRequestDto>>(nearbyRequests);

            return new PaginationResponse<PharmacyNearbyRequestDto>(
                index: queryParams.PageIndex,
                size: queryParams.PageSize,
                total: totalCount,
                data: mappedRequests
            );
        }

        public async Task<PrescriptionRequestDto> GetRequestDetailsForPharmacyAsync(int requestId)
        {
            var repo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            var spec = new PrescriptionRequestWithDetailsForPharmacySpec(requestId);

            var request = await repo.GetByIdWithSpecAsync(spec);

            if (request == null)
                throw new RequestNotFoundException("Prescription Request Not Found");

            return mapper.Map<PrescriptionRequestDto>(request);
        }
        #endregion

        #region Admin Services
        public async Task<PaginationResponse<AdminPrescriptionRequestDto>> GetAllPlatformRequestsAsync(PrescriptionRequestQueryParams queryParams)
        {
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            var dataSpec = new AdminRequestsSpec(queryParams);
            var countSpec = new AdminRequestsCountSpec(queryParams);

            var requests = await requestRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await requestRepo.GetCountAsync(countSpec);

            var mappedRequests = mapper.Map<IReadOnlyList<AdminPrescriptionRequestDto>>(requests);

            return new PaginationResponse<AdminPrescriptionRequestDto>(
                index: queryParams.PageIndex,
                size: queryParams.PageSize,
                total: totalCount,
                data: mappedRequests
            ); 
        }

        public async Task<AdminPrescriptionRequestDetailsDto> GetAdminRequestDetailsAsync(int requestId)
        {
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var spec = new AdminRequestDetailsSpec(requestId);

            var requestDetails = await requestRepo.GetByIdWithSpecAsync(spec);

            if (requestDetails == null)
                throw new RequestNotFoundException("Prescription Request Not Found");

            return mapper.Map<AdminPrescriptionRequestDetailsDto>(requestDetails);
        }


        #endregion
    }
}