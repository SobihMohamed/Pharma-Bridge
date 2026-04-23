using AutoMapper;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Exceptions.NotFoundHandeler.Request;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.Dto_s.Attachment;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.ServicesImplementation.PrescriptionRequest
{
    public class PrescriptionRequestService(IUnitOfWork unitOfWork, IMapper mapper, IAttachementService attachementService ) : IPrescriptionRequestService
    {
        #region Helper Methods In CreateRequest Service
        private void ValidateRequestInput(CreatePrescriptionRequestDto requestDto) 
        {
            // Business Rule : Not Valid if Null of Medicin Name and Image Url 
            bool hasMedicineName = !string.IsNullOrWhiteSpace(requestDto.MedicineName);

            bool hasImage = requestDto.ImageUrl != null && requestDto.ImageUrl.Length > 0;

            if (!hasMedicineName && !hasImage)
                throw new BadRequestCustomeException("Please provide at least a medicine name or upload an image of the prescription.");
        }
        // 💡 زودنا applicationUserId كبارامتر تالت
        private PrescriptionRequestEntity BuidPrescriptionRequestEntity(CreatePrescriptionRequestDto createRequestDto, string actualPatientProfileId, string applicationUserId)
        {
            var request = mapper.Map<PrescriptionRequestEntity>(createRequestDto);

            request.PatientProfileId = actualPatientProfileId;
            request.Status = PrescriptionStatus.Pending;
            request.ExpiresAt = DateTime.UtcNow.AddHours(24);
            request.CreatedAt = DateTime.UtcNow;

            request.PrescriptionRequestHistorys.Add(new PrescriptionRequestHistory
            {
                OldStatus = null,
                NewStatus = PrescriptionStatus.Pending,
                ChangedAt = DateTime.UtcNow,
                Notes = "Prescription Request Created by Patient",
                // user id of the patient who created the request
                ChangedById = applicationUserId,
            });
            return request;
        }
        private async Task<PatientAddress> GetValidAddressAsync(int addressId, string patientId)
        {
            var addressSpec = new PatientAddressWithPatientprofileSpec(addressId, patientId);
            var addressRepo = unitOfWork.GetRepository<PatientAddress, int>();

            var deliveryAddress = await addressRepo.GetByIdWithSpecAsync(addressSpec);
            if (deliveryAddress == null) throw new BadRequestCustomeException("Invalid delivery address");

            return deliveryAddress; 
        }
        #endregion
        public async Task<PrescriptionRequestDto> CreateRequestAsync(CreatePrescriptionRequestDto createDto, Guid userIdFromToken)
        {
            var applicationUserId = userIdFromToken.ToString();

            var profileSpec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var patientProfile = await unitOfWork.GetRepository<PatientProfile, string>().GetByIdWithSpecAsync(profileSpec);

            if (patientProfile == null)
                throw new BadRequestCustomeException("Patient profile not found for this user.");

            // 1 - validate the input form user 
            var actualPatientProfileId = patientProfile.Id;
            ValidateRequestInput(createDto);

            // 2 - validate the delivery address id exist and belong to the patient
            var patientAddress = await GetValidAddressAsync(createDto.DeliveryAddressId, actualPatientProfileId);
            // Image Functionality : Upload the image to the server and get the URL (if provided)
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
            // 3 - create the Prescription Request Object to send to Db 
            var request = BuidPrescriptionRequestEntity(createDto, actualPatientProfileId, applicationUserId);
            request.ImageUrl = uploadedImageUrl;
            // 4 - Get the Repo of the request
            await unitOfWork.GetRepository<PrescriptionRequestEntity, int>().AddAsync(request);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0) throw new BadRequestCustomeException("Failed to create prescription request");

            // 5 - Map the result to PrescriptionRequestDto to send it to client 
            var requestDto = mapper.Map<PrescriptionRequestDto>(request);
            requestDto.DeliveryArea = $"{patientAddress.City} - {patientAddress.AddressLine}";
            return requestDto;

        }
        public async Task<PaginationResponse<PrescriptionRequestDto>> GetPatientRequestsAsync(Guid userIdFromToken, PrescriptionRequestQueryParams queryParams)
        {
            var applicationUserId = userIdFromToken.ToString();
            var profileSpec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var patientProfile = await unitOfWork.GetRepository<PatientProfile, string>().GetByIdWithSpecAsync(profileSpec);

            if (patientProfile == null)
                throw new BadRequestCustomeException("Patient profile not found for this user.");
            
            var actualPatientProfileId = patientProfile.Id;
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            // create the specification 
            var dataSpec = new PatientRequestWithAddressandBidsSpec(actualPatientProfileId, queryParams);
            var countSpec = new PatientRequestWithAddressandBidsCountSpec(actualPatientProfileId, queryParams);

            // excute queries in database 
            var requests = await requestRepo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await requestRepo.GetCountAsync(countSpec);

            // map the result to Dto
            var mappedRequests = mapper.Map<IReadOnlyList<PrescriptionRequestDto>>(requests);
            return new PaginationResponse<PrescriptionRequestDto>
            (
                index :queryParams.PageIndex,
                size : queryParams.PageSize,
                total : totalCount,
                data : mappedRequests
            );
        }

        public async Task<PrescriptionRequestDetailsDto> GetPatientRequestDetailsAsync(int requestId, Guid userIdFromToken)
        {
            var applicationUserId = userIdFromToken.ToString();
            var profileSpec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var patientProfile = await unitOfWork.GetRepository<PatientProfile, string>().GetByIdWithSpecAsync(profileSpec);

            if (patientProfile == null)
                throw new BadRequestCustomeException("Patient profile not found for this user.");

            var actualPatientProfileId = patientProfile.Id;

            // spec 
            var spec = new PatientRequestDetailsWithIncludesSpec(requestId, actualPatientProfileId);

            // rep 
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var requestDetails = await requestRepo.GetByIdWithSpecAsync(spec);

            if (requestDetails == null)
            {
                throw new RequestNotFoundException("Prescription Request Not Found");
            }

            return mapper.Map<PrescriptionRequestDetailsDto>(requestDetails);
        }

    }
}
