using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Exceptions.NotFoundHandeler.Request;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Request;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.ServicesImplementation.PrescriptionRequest
{
    public partial class PrescriptionRequestService
    {
        private async Task<string> GetValidPatientProfileIdAsync(string applicationUserId)
        {
            var profileSpec = new PatientProfileByAppUserIdSpec(applicationUserId);
            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var patientProfile = await patientRepo.GetByIdWithSpecAsync(profileSpec);

            if (patientProfile == null)
                throw new BadRequestCustomeException("Patient profile not found for this user.");

            return patientProfile.Id;
        }

        private void ValidateRequestInput(CreatePrescriptionRequestDto requestDto)
        {
            bool hasMedicineName = !string.IsNullOrWhiteSpace(requestDto.MedicineName);
            bool hasImage = requestDto.ImageUrl != null && requestDto.ImageUrl.Length > 0;

            if (!hasMedicineName && !hasImage)
                throw new BadRequestCustomeException("Please provide at least a medicine name or upload an image of the prescription.");
        }

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
                ChangedById = applicationUserId,
            });
            return request;
        }

        private async Task<Domain.Models.User.PatientAddress> GetValidAddressAsync(int addressId, string patientId)
        {
            var addressSpec = new PatientAddressWithPatientprofileSpec(addressId, patientId);
            var addressRepo = unitOfWork.GetRepository<Domain.Models.User.PatientAddress, int>();

            var deliveryAddress = await addressRepo.GetByIdWithSpecAsync(addressSpec);
            if (deliveryAddress == null) throw new BadRequestCustomeException("Invalid delivery address");

            return deliveryAddress;
        }

        private async Task<PrescriptionRequestEntity> GetValidRequestForCancellationAsync(int requestId, string patientProfileId)
        {
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            var spec = new RequestWithBidsSpec(requestId);
            var request = await requestRepo.GetByIdWithSpecAsync(spec);

            if (request == null)
                throw new RequestNotFoundException("Prescription Request Not Found");

            if (request.PatientProfileId != patientProfileId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this request.");

            if (request.Status != PrescriptionStatus.Pending && request.Status != PrescriptionStatus.HasBids)
                throw new BadRequestCustomeException("You cannot cancel this request at this stage. You might have already accepted a bid.");

            return request;
        }
       
        private void ProcessRequestCancellation(PrescriptionRequestEntity request, string applicationUserId)
        {
            var oldStatus = request.Status;

            request.Status = PrescriptionStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;

            if (request.Bids != null && request.Bids.Any())
            {
                foreach (var bid in request.Bids)
                {
                    if (bid.Status == BidStatus.Pending)
                    {
                        bid.Status = BidStatus.Rejected;
                        bid.Notes = "Auto-rejected because the patient cancelled the prescription request.";
                    }
                }
            }

            request.PrescriptionRequestHistorys.Add(new PrescriptionRequestHistory
            {
                OldStatus = oldStatus,
                NewStatus = PrescriptionStatus.Cancelled,
                ChangedAt = DateTime.UtcNow,
                Notes = "Prescription Request and related bids cancelled by Patient",
                ChangedById = applicationUserId
            });

            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            requestRepo.UpdateAsync(request);
        }

        private async Task<Domain.Models.Pharma_Requests.Pharmacy> GetValidPharmacyAsync(int pharmacyId)
        {
            var pharmacyRepo = unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);

            if (pharmacy == null)
                throw new BadRequestCustomeException("Pharmacy profile not found.");

            return pharmacy;
        }
    }

}