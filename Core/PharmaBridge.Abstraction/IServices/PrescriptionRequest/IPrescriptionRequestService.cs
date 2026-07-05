using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.DTOs.PharmaRequests.AdminReq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.PrescriptionRequest
{
    public interface IPrescriptionRequestService
    {
        // --- Patient (Client) Operations ---

        // Creates a new request (uploads prescription / writes med names) and broadcasts it to nearby pharmacies.
        Task<PrescriptionRequestDto> CreateRequestAsync(CreatePrescriptionRequestDto createDto, Guid patientId);

        // Patient can view all their past and current requests with filtering.
        Task<PaginationResponse<PrescriptionRequestDto>> GetPatientRequestsAsync(Guid patientId, PrescriptionRequestQueryParams queryParams);

        // View full details of a specific request (including attached images/notes).
        Task<PrescriptionRequestDetailsDto> GetPatientRequestDetailsAsync(int requestId, Guid patientId);

        // Patient can cancel the request BEFORE accepting any bids.
        Task<bool> CancelRequestAsync(int requestId, Guid patientId);


        // --- Pharmacy (Provider) Operations ---
        // Pharmacy views available requests within their range (e.g., 5km) based on their location. (Masked patient data).
        Task<PaginationResponse<PharmacyNearbyRequestDto>> GetNearbyRequestsAsync(int pharmacyId, PrescriptionRequestQueryParams queryParams);
        Task<PrescriptionRequestDto> GetRequestDetailsForPharmacyAsync(int requestId);

        // --- Admin Operations ---
        // Admin can monitor all platform requests for auditing and support.
        Task<PaginationResponse<AdminPrescriptionRequestDto>> GetAllPlatformRequestsAsync(PrescriptionRequestQueryParams queryParams);
        Task<AdminPrescriptionRequestDetailsDto> GetAdminRequestDetailsAsync(int requestId);

    }
}

/*
// --- Query Params Classes (To be placed in Shared/Common/params/Requests folder) ---

public class PrescriptionRequestQueryParams : BaseQueryParams
{
    // e.g., Pending, HasBids, Closed, Cancelled
    public RequestStatus? Status { get; set; } 
    
    // For date filtering
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    // Specifically for Pharmacy to filter by distance (Defaults to 5km if not provided)
    public double? RadiusInKm { get; set; } = 5.0; 
    
    // For Admin to filter requests of a specific patient
    public Guid? PatientId { get; set; } 
}
*/