using AutoMapper;
using PharmaBridge.Abstraction.IServices.Complaint;
using PharmaBridge.Domain.Contracts.GenericReposPattern;
using PharmaBridge.Domain.Contracts.SpecificationPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Services.Specifications;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Complaint;
using PharmaBridge.Shared.DTOs.Complaint;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
namespace PharmaBridge.Services.ServicesImplementation.Complaint
{
    public class ComplaintService(IUnitOfWork unitOfWork, IMapper mapper) : IComplaintService
    {
        //Patient Operations
        public async Task<ComplaintDetailsDto> SubmitComplaintAsync(CreateComplaintDto createDto, Guid patientId)
        {
            var userId = patientId.ToString();

            ValidateRequestInput(createDto);
            var complaint = BuildComplaintEntity(createDto, userId);
            await unitOfWork.GetRepository<PharmaBridge.Domain.Models.UserAccess.Complaint, int>().AddAsync(complaint);
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0) throw new BadRequestCustomeException("Failed to submit complaint");
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
            var newStatus = ParseAndValidateStatus(updateDto.Status);
            ValidateResolutionNotes(newStatus, updateDto.AdminNotes);
            ApplyStatusUpdates(complaint, newStatus, updateDto.AdminNotes, adminId);
            var repo = GetComplaintRepo();
            repo.UpdateAsync(complaint);
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0)
                throw new BadRequestCustomeException("Failed to save the updated complaint status to the database.");
            return true;
        }

        //public async Task<bool> UpdateComplaintStatusAsync(Guid complaintId, UpdateComplaintStatusDto updateDto)
        //{
        //    var repo = GetComplaintRepo();

        //    // 1. بما إن الـ Interface بيجبرنا نستخدم Guid، والـ Repo بيحتاج int،
        //    // هنستخدم Specification عشان نبحث عن الشكوى بالـ Guid بدل GetByIdAsync
        //    var spec = new ComplaintByGuidSpec(complaintId);
        //    var complaint = await repo.GetByIdWithSpecAsync(spec);

        //    if (complaint == null)
        //        throw new NotFoundCutomeException($"Complaint with ID {complaintId} was not found on the platform.");

        //    // 2. Parse Enum Status (باستخدام الـ Helper Method اللي عملناها)
        //    var newStatus = ParseAndValidateStatus(updateDto.Status);

        //    // 3. Validate Business Rules
        //    ValidateResolutionNotes(newStatus, updateDto.AdminNotes);

        //    // 4. Apply Updates
        //    complaint.Status = newStatus;

        //    if (!string.IsNullOrWhiteSpace(updateDto.AdminNotes))
        //    {
        //        complaint.AdminNotes = updateDto.AdminNotes;
        //    }

        //    if (newStatus == ComplaintStatus.Resolved || newStatus == ComplaintStatus.Rejected)
        //    {
        //        complaint.ResolvedAt = DateTime.UtcNow;

        //        // لحل مشكلة الـ adminId من غير ما نغير الـ Interface،
        //        // المفروض يتم حقن IHttpContextAccessor في الـ Constructor بتاع الـ Service
        //        // وتقرأ الـ ID الخاص بالأدمن من الـ Token كالتالي (متروكة كتعليق لتشغيل الكود الآن):

        //        // complaint.ResolvedById = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        //    }

        //    repo.UpdateAsync(complaint);

        //    var result = await unitOfWork.SaveChangesAsync();
        //    if (result <= 0)
        //        throw new BadRequestCustomeException("Failed to save the updated complaint status to the database.");

        //    return true;
        //}

        #region Helper Methods In Complaint Service
        private void ValidateRequestInput(CreateComplaintDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.Title) || string.IsNullOrEmpty(requestDto.Description))
            {
                throw new BadRequestCustomeException("Please Write the Title and Description");
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
        private ComplaintStatus ParseAndValidateStatus(string statusString)
        {
            if (!Enum.TryParse<ComplaintStatus>(statusString, true, out var parsedStatus))
            {
                throw new BadRequestCustomeException("Invalid complaint status format.");
            }
            return parsedStatus;
        }
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
        #endregion
    }
}
