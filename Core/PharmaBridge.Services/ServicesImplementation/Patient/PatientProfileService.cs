using AutoMapper;
using PharmaBridge.Abstraction.IServices.PatientProfiles;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.Patient;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;


namespace PharmaBridge.Services.ServicesImplementation.Patient
{
    public class PatientProfileService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientProfileService
    {
        public async Task<PatientProfileDetailsDto> GetMyProfileAsync(string applicationUserId)
        {
            var spec = new PatientCheckAndGetInfoSpec(applicationUserId);
            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var patient = await patientRepo.GetByIdWithSpecAsync(spec);

            // Lazy Initialization
            if (patient == null)
            {
                var newProfile = new PatientProfile
                {
                    ApplicationUserId = applicationUserId,
                    Id = Guid.NewGuid().ToString()
                };

                await patientRepo.AddAsync(newProfile);

                if (await unitOfWork.SaveChangesAsync() <= 0)
                    throw new BadRequestCustomeException("Failed to initialize patient profile.");

                patient = await patientRepo.GetByIdWithSpecAsync(spec);
            }

            var dto = mapper.Map<PatientProfileDetailsDto>(patient!);
            var patientId = patient!.Id;

            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var ratingRepo = unitOfWork.GetRepository<PharmacyRating, int>();
            var complaintRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.UserAccess.Complaint, int>();

            dto.TotalPrescriptionRequests = await requestRepo.GetCountAsync(new PatientRequestsCountSpec(patientId));
            dto.TotalPharmacyRatings = await ratingRepo.GetCountAsync(new PatientRatingsCountSpec(patientId));
            dto.ComplaintsSubmitted = await complaintRepo.GetCountAsync(new PatientComplaintsCountSpec(patientId));

            dto.OrdersCount = await orderRepo.GetCountAsync(new PatientOrdersCountSpec(patientId));
            dto.PendingOrders = await orderRepo.GetCountAsync(new PatientOrdersCountSpec(patientId, OrderStatus.Pending));
            dto.CompletedOrders = await orderRepo.GetCountAsync(new PatientOrdersCountSpec(patientId, OrderStatus.Completed));
            dto.CancelledOrders = await orderRepo.GetCountAsync(new PatientOrdersCountSpec(patientId, OrderStatus.Cancelled));

            return dto;
        }

        public async Task<PatientProfileDetailsDto> UpdateMyProfileAsync(string applicationUserId, PatientProfileToUpdateDto updateDto)
        {
            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var spec = new PatientProfileWithDetailsSpec(applicationUserId);
            var patient = await patientRepo.GetByIdWithSpecAsync(spec);

            if (patient == null)
                throw new NotFoundCutomeException("Patient not found");

            if (string.IsNullOrWhiteSpace(updateDto.FullName))
                throw new BadRequestCustomeException("Full name cannot be empty");

            patient.ApplicationUser.FullName = updateDto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
            {
                patient.ApplicationUser.PhoneNumber = updateDto.PhoneNumber;
            }

            patientRepo.UpdateAsync(patient);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                throw new BadRequestCustomeException("Failed to update patient profile");

            return mapper.Map<PatientProfileDetailsDto>(patient);
        }

        public async Task<PaginationResponse<PatientProfileDto>> GetAllPatientsAsync(PatientQueryParams queryParams)
        {
            if (queryParams.PageIndex <= 0)
                queryParams.PageIndex = 1;

            if (queryParams.PageSize <= 0 || queryParams.PageSize > 50)
                queryParams.PageSize = 10;

            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var dataSpec = new PatientWithFiltersSpec(queryParams);
            var countSpec = new PatientWithFiltersSpec(queryParams.Search);

            var patients = await patientRepo.GetAllWithSpecAsync(dataSpec);
            var totalItems = await patientRepo.GetCountAsync(countSpec);

            var data = mapper.Map<IReadOnlyList<PatientProfileDto>>(patients);

            return new PaginationResponse<PatientProfileDto>(
                queryParams.PageIndex,
                queryParams.PageSize,
                totalItems,
                data
            );
        }
        public async Task<PatientProfileDetailsDto> GetPatientByApplicationUserIdAsync(string patientProfileId)
        {
            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();

            var spec = new PatientProfileWithDetailsSpec(patientProfileId);
            var patient = await patientRepo.GetByIdWithSpecAsync(spec);

            if (patient is null)
                throw new NotFoundCutomeException($"Patient with profile id '{patientProfileId}' was not found.");

            return mapper.Map<PatientProfileDetailsDto>(patient);
        }
    }
}
    
