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
        //Task<ComplaintDetailsDto> SubmitComplaintAsync(CreateComplaintDto createDto, Guid patientId);

        // Patient views the status of their own submitted complaints.
        //Task<Pagination<ComplaintDetailsDto>> GetPatientComplaintsAsync(Guid patientId);


        // =========================================================================
        // --- Admin Operations ---
        // =========================================================================

        // Admin views all platform complaints to manage them.
        //Task<Pagination<ComplaintDto>> GetAllPlatformComplaintsAsync(ComplaintQueryParams queryParams);

        // Admin views full details of a specific complaint (including attached evidence).
        //Task<ComplaintDetailsDto> GetComplaintDetailsAsync(int complaintId);

        /// Admin updates the status of the complaint and optionally adds resolution notes and actions taken.
        //Task<bool> UpdateComplaintStatusAsync(Guid complaintId, UpdateComplaintStatusDto updateDto);
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