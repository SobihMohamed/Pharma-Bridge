using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PharmaBridge.Abstraction.IServices.Complaint;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.SpecificationPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Complaint;
using PharmaBridge.Shared.DTOs.Complaint;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
namespace PharmaBridge.Services.ServicesImplementation.Complaint
{
    public class ComplaintService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : IComplaintService
    {
        //Patient Operations
        public async Task<ComplaintDetailsDto> SubmitComplaintAsync(CreateComplaintDto createDto, Guid patientId)
        {
            var userId = patientId.ToString();

            ValidateRequestInput(createDto);
            await ValidateOptionalOrderAsync(createDto.OrderId);
            var complaint = BuildComplaintEntity(createDto, userId);
            await unitOfWork.GetRepository<PharmaBridge.Domain.Models.UserAccess.Complaint, int>().AddAsync(complaint);
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0) throw new BadRequestCustomeException("Failed to submit complaint");
            var adminIds = await GetAdminUserIdsAsync(); 

            if (adminIds != null && adminIds.Any())
            {
                // 2. Prepare the notification message
                string bodyMessage = createDto.OrderId.HasValue && createDto.OrderId > 0
                    ? $"A new complaint has been submitted regarding Order #{createDto.OrderId}. Title: {createDto.Title}"
                    : $"A new general complaint has been submitted. Title: {createDto.Title}";

                foreach (var adminId in adminIds)
                {
                    var message = new NotificationContentDto
                    {
                        UserId = adminId,
                        Subject = "New Complaint Submitted ⚠️",
                        Body = bodyMessage,
                        ReferenceId = complaint.Id, 
                        Payload = null
                    };

                    await notificationService.SendNotificationAsync(message, Shared.EnumHelper.NotificationEnums.NotificationType.Push);
                }
            }
            var complaintDetailsDto = mapper.Map<ComplaintDetailsDto>(complaint);
            return complaintDetailsDto;            
        }
        public async Task<PaginationResponse<ComplaintDetailsDto>> GetPatientComplaintsAsync(Guid patientId, int pageSize, int pageIndex)
        {
            var userId = patientId.ToString();

            var countSpec = new PatientComplaintsCountSpec(userId);
            var dataSpec = new PatientComplaintsWithPaginationSpec(userId, pageSize, pageIndex);

            var repo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.UserAccess.Complaint, int>();
            var totalItems = await repo.GetCountAsync(countSpec);
            var complaints = await repo.GetAllWithSpecAsync(dataSpec);
            var mappedData = mapper.Map<IReadOnlyList<ComplaintDetailsDto>>(complaints);

            return new PaginationResponse<ComplaintDetailsDto>(pageIndex, pageSize, totalItems, mappedData);
        }

        //Admin Operations

        public async Task<PaginationResponse<ComplaintDto>> GetAllPlatformComplaintsAsync(ComplaintQueryParams queryParams)
        {
            await ValidatePharmacyExistsAsync(queryParams.PharmacyId);
            var countSpec = new AllComplaintsCountSpec(queryParams);
            var dataSpec = new AllComplaintsWithPaginationSpec(queryParams);

            var repo = GetComplaintRepo();

            var totalItems = await repo.GetCountAsync(countSpec);
            var complaints = await repo.GetAllWithSpecAsync(dataSpec);

            var mappedData = mapper.Map<IReadOnlyList<ComplaintDto>>(complaints);

            return new PaginationResponse<ComplaintDto>(queryParams.PageIndex, queryParams.PageSize, totalItems, mappedData);
        }
        public async Task<ComplaintDetailsDto> GetComplaintDetailsAsync(int complaintId)
        {
            var spec = new ComplaintDetailsWithIncludesSpec(complaintId);
            var complaint = await GetComplaintOrThrowAsync(complaintId, spec);
            return mapper.Map<ComplaintDetailsDto>(complaint);
        }
        public async Task<bool> UpdateComplaintStatusAsync(int complaintId, UpdateComplaintStatusDto updateDto, string adminId)
        {
            var complaint = await GetComplaintOrThrowAsync(complaintId);
            var newStatus = updateDto.Status;
            ValidateResolutionNotes(newStatus, updateDto.AdminNotes);
            ApplyStatusUpdates(complaint, newStatus, updateDto.AdminNotes, adminId);
            var repo = GetComplaintRepo();
            repo.UpdateAsync(complaint);
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0)
                throw new BadRequestCustomeException("Failed to save the updated complaint status to the database.");
            return true;
        }

        private async Task<List<string>> GetAdminUserIdsAsync()
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");

            return admins.Select(admin => admin.Id).ToList();
        }
        #region Helper Methods In Complaint Service
        private void ValidateRequestInput(CreateComplaintDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.Title) || string.IsNullOrEmpty(requestDto.Description))
            {
                throw new BadRequestCustomeException("Please Write the Title and Description");
            }
        }
        private async Task ValidateOptionalOrderAsync(int? orderId)
        {
            if (orderId.HasValue)
            {
                if (orderId.Value <= 0)
                    throw new BadRequestCustomeException("Invalid Order ID. It must be greater than zero.");

                var orderRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.UserAccess.Order, int>();
                var orderExists = await orderRepo.GetByIdAsync(orderId.Value);

                if (orderExists == null)
                {
                    throw new NotFoundCutomeException($"Order with ID '{orderId.Value}' does not exist. Cannot link complaint to a non-existent order.");
                }
            }
        }

        private PharmaBridge.Domain.Models.UserAccess.Complaint BuildComplaintEntity(CreateComplaintDto createDto, string patientId)
        {
            var complaint = mapper.Map<PharmaBridge.Domain.Models.UserAccess.Complaint>(createDto);
            complaint.Status = ComplaintStatus.Pending;
            complaint.SubmittedById = patientId;
            return complaint;
        }
        #endregion

        #region Private Helper Methods Admin Operations
        private IGenericRepo<Domain.Models.UserAccess.Complaint, int> GetComplaintRepo()
        {
            return unitOfWork.GetRepository<Domain.Models.UserAccess.Complaint, int>();
        }
        private void ValidateResolutionNotes(ComplaintStatus status, string? adminNotes)
        {
            bool isClosingComplaint = status == ComplaintStatus.Resolved || status == ComplaintStatus.Rejected;

            if (isClosingComplaint && string.IsNullOrWhiteSpace(adminNotes))
            {
                throw new BadRequestCustomeException("Resolution notes or 'Actions Taken' are strictly required when resolving or rejecting a complaint.");
            }
        }
        private void ApplyStatusUpdates(Domain.Models.UserAccess.Complaint complaint, ComplaintStatus newStatus, string? adminNotes, string adminId)
        {
            complaint.Status = newStatus;

            if (!string.IsNullOrWhiteSpace(adminNotes))
            {
                complaint.AdminNotes = adminNotes;
            }
            if (newStatus == ComplaintStatus.Resolved || newStatus == ComplaintStatus.Rejected)
            {
                complaint.ResolvedAt = DateTime.UtcNow;
                complaint.ResolvedById = adminId;
            }
        }
        private async Task<Domain.Models.UserAccess.Complaint> GetComplaintOrThrowAsync(int id, ISpecifications<Domain.Models.UserAccess.Complaint, int>? spec = null)
        {
            var repo = GetComplaintRepo();
            Domain.Models.UserAccess.Complaint? complaint;

            if (spec != null)
                complaint = await repo.GetByIdWithSpecAsync(spec);
            else
                complaint = await repo.GetByIdAsync(id);

            if (complaint == null)
                throw new NotFoundCutomeException($"Complaint with ID {id} was not found on the platform.");

            return complaint;
        }

        private async Task ValidatePharmacyExistsAsync(int? pharmacyId)
        {
            if (pharmacyId.HasValue)
            {
                var pharmacyRepo = unitOfWork.GetRepository<PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy, int>();
                var pharmacyExists = await pharmacyRepo.GetByIdAsync(pharmacyId.Value);

                if (pharmacyExists == null)
                {
                    throw new NotFoundCutomeException($"Pharmacy with ID '{pharmacyId.Value}' does not exist on the platform.");
                }
            }
        }
        #endregion
    }
}
