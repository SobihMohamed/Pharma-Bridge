
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Order;

namespace PharmaBridge.Abstraction.IServices.Order
{
    // This interface defines the contract for the Order workflow.
    public interface IOrderService
    {
        // =========================================================================
        // --- System / Internal Operations (Not exposed directly to Controllers) ---
        // =========================================================================

        // Triggered automatically by IBidService when a patient accepts a bid.
        // It takes the winning Bid details, extracts the items and prices, and creates a new Order.
        Task<OrderDetailsDto> CreateOrderFromBidAsync(int winningBidId);


        // =========================================================================
        // --- Pharmacy (Provider) Operations ---
        // =========================================================================
        Task<PaginationResponse<OrderDto>> GetPharmacyOrdersAsync(int pharmacyId, OrderQueryParams queryParams);
        Task<OrderDetailsDto> GetPharmacyOrderDetailsAsync(int orderId, int pharmacyId);
        Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto, int pharmacyId);


        // =========================================================================
        // --- Patient (Client) Operations ---
        // =========================================================================
        Task<PaginationResponse<OrderDto>> GetPatientOrdersAsync(Guid patientId, OrderQueryParams queryParams);
        Task<OrderDetailsDto> GetPatientOrderDetailsAsync(int orderId, Guid patientId);


        // =========================================================================
        // --- Admin Operations (Read-Only Observer) ---
        // =========================================================================
        Task<PaginationResponse<OrderDto>> GetAllPlatformOrdersAsync(OrderQueryParams queryParams);
        Task<AdminOrderDetailsDto> GetAdminOrderDetailsAsync(int orderId);
    }
}
/*
// --- Query Params Classes (To be placed in Shared/Common/Params/Order folder) ---

public class OrderQueryParams : BaseQueryParams
{
    // e.g., Preparing, OutForDelivery, Completed, Cancelled
    public OrderStatus? Status { get; set; } 
    
    // For date filtering (e.g., "This Month" revenue)
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    // For Admin: filtering by a specific pharmacy or patient
    public Guid? PharmacyId { get; set; } 
    public Guid? PatientId { get; set; } 
}
*/