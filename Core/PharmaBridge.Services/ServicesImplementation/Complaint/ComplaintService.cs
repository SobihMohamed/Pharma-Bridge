using PharmaBridge.Services.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.ServicesImplementation.Complaint
{
    public class ComplaintService(IUnitOfWork unitOfWork, IMapper mapper) : IComplaintService
    {
        //Patient Operations
        public async Task<ComplaintDetailsDto> SubmitComplaintAsync(CreateComplaintDto createDto, Guid patientId)
        {
            var userId = patientId.ToString();

            ValidateComplaintInput(createDto);
            var complaint = BuildComplaintEntity(createDto, userId);
            await unitOfWork.GetRepository<Complaint, int>().AddAsync(complaint);
            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0) throw new BadRequestCustomeException("Failed to submit complaint");
            var complaintDetailsDto = mapper.Map<ComplaintDetailsDto>(complaint);
            return complaintDetailsDto;            
        }
        public async Task<Pagination<ComplaintDetailsDto>> GetPatientComplaintsAsync(Guid patientId, int pageSize, int pageIndex)
        {
            var userId = patientId.ToString();

            var countSpec = new PatientComplaintsCountSpec(userId);
            var dataSpec = new PatientComplaintsWithPaginationSpec(userId, pageSize, pageIndex);

            var repo = unitOfWork.GetRepository<Complaint, int>();
            var totalItems = await repo.GetAllAsync(countSpec);
            var complaints = await repo.GetAllWithSpecAsync(dataSpec);
            var mappedData = mapper.Map<IReadOnlyList<ComplaintDetailsDto>>(complaints);
            return new Pagination<ComplaintDetailsDto>(pageIndex, pageSize, totalItems, mappedData);
        }

        //Admin Operations
        public async Task<Pagination<ComplaintDto>> GetAllPlatformComplaintsAsync(ComplaintQueryParams queryParams)
        {
            var countSpec = new AllComplaintsCountSpec(queryParams);
            var dataSpec = new AllComplaintsWithPaginationSpec(queryParams);

            var repo = GetComplaintRepo();

            var totalItems = await repo.GetCountAsync(countSpec);
            var complaints = await repo.GetAllWithSpecAsync(dataSpec);

            var mappedData = mapper.Map<IReadOnlyList<ComplaintDto>>(complaints);

            return new Pagination<ComplaintDto>(queryParams.PageIndex, queryParams.PageSize, totalItems, mappedData);
        }

        public async Task<ComplaintDetailsDto> GetComplaintDetailsAsync(int complaintId)
        {
            var spec = new ComplaintDetailsWithIncludesSpec(complaintId);
            var complaint = await GetComplaintOrThrowAsync(complaintId, spec);
            return mapper.Map<ComplaintDetailsDto>(complaint);
        }

        public async Task<bool> UpdateComplaintStatusAsync(Guid complaintId, UpdateComplaintStatusDto updateDto)
        {
            var complaint = await GetComplaintOrThrowAsync(complaintId);
            bool isClosingComplaint = updateDto.Status == ComplaintStatus.Resolved || updateDto.Status == ComplaintStatus.Rejected;
            
            if (isClosingComplaint && string.IsNullOrWhiteSpace(updateDto.AdminNotes))
            {
                throw new BadRequestCustomeException("Resolution notes or 'Actions Taken' are strictly required when resolving or rejecting a complaint.");
            }
            complaint.Status = updateDto.Status;
            
            if (!string.IsNullOrWhiteSpace(updateDto.AdminNotes))
            {
                complaint.AdminNotes = updateDto.AdminNotes;
            }
            
            if (updateDto.Status == ComplaintStatus.Resolved)
            {
                complaint.ResolvedAt = DateTime.UtcNow;
                complaint.ResolvedById = adminId;
            }
            var repo = GetComplaintRepo();
            repo.UpdateAsync(complaint);

            var result = await unitOfWork.SaveChangesAsync();
            if (result <= 0)
                throw new BadRequestCustomeException("Failed to save the updated complaint status to the database.");

            return true;
        }

        #region Helper Methods In Complaint Service
        private void ValidateRequestInput(CreateComplaintDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.Title) && string.IsNullOrEmpty(requestDto.Description))
            {
                throw new BadRequestCustomeException("Please Write the Title and Description");
            }
        }
        private Complaint BuildComplaintEntity(CreateComplaintDto createDto, string patientId)
        {
            var complaint = mapper.Map<Complaint>(createDto);
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

        private async Task<Domain.Models.UserAccess.Complaint> GetComplaintOrThrowAsync(int id, ISpecifications<Domain.Models.UserAccess.Complaint, int>? spec = null)
        {
            var repo = GetComplaintRepo();
            Domain.Models.UserAccess.Complaint? complaint;

            if (spec != null)
                complaint = await repo.GetByIdWithSpecAsync(spec);
            else
                complaint = await repo.GetByIdAsync(id);

            if (complaint == null)
                throw new NotFoundCustomeException($"Complaint with ID {id} was not found on the platform.");

            return complaint;
        }

        #endregion
    }
}
