using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Complaint;
using PharmaBridge.Shared.DTOs.Complaint;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Complaint
{
    // This interface defines the contract for the Dispute and Complaint system.
    // Handles reports submitted by patients against orders/pharmacies and Admin resolutions.
    public interface IComplaintService
    {
        // =========================================================================
        // --- Patient (Client) Operations ---
        // =========================================================================

        // Patient submits a complaint against a specific Order/Pharmacy.
        Task<ComplaintDetailsDto> SubmitComplaintAsync(CreateComplaintDto createDto, Guid patientId);

        // Patient views the status of their own submitted complaints.
        Task<PaginationResponse<ComplaintDetailsDto>> GetPatientComplaintsAsync(Guid patientId, int pageSize, int pageIndex);


        // =========================================================================
        // --- Admin Operations ---
        // =========================================================================

        // Admin views all platform complaints to manage them.
        Task<PaginationResponse<ComplaintDto>> GetAllPlatformComplaintsAsync(ComplaintQueryParams queryParams);

        // Admin views full details of a specific complaint (including attached evidence).
        Task<ComplaintDetailsDto> GetComplaintDetailsAsync(int complaintId);

        /// Admin updates the status of the complaint and optionally adds resolution notes and actions taken.
        Task<bool> UpdateComplaintStatusAsync(int complaintId, UpdateComplaintStatusDto updateDto, string adminId);
    }
}

/*
// --- Query Params Classes ---
public class ComplaintQueryParams : BaseQueryParams
{
    // e.g., Open, InReview, Resolved
    public ComplaintStatus? Status { get; set; } 
    public Guid? PharmacyId { get; set; }
}
*/