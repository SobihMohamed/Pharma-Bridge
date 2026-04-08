
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
        //Task<OrderDetailsDto> CreateOrderFromBidAsync(Guid winningBidId);


        // =========================================================================
        // --- Pharmacy (Provider) Operations ---
        // =========================================================================
        //Task<Pagination<OrderDto>> GetPharmacyOrdersAsync(Guid pharmacyId, OrderQueryParams queryParams);
        //Task<OrderDetailsDto> GetPharmacyOrderDetailsAsync(Guid orderId, Guid pharmacyId);
        //Task<bool> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto updateStatusDto, Guid pharmacyId);


        // =========================================================================
        // --- Patient (Client) Operations ---
        // =========================================================================
        //Task<Pagination<OrderDto>> GetPatientOrdersAsync(Guid patientId, OrderQueryParams queryParams);
        //Task<OrderDetailsDto> GetPatientOrderDetailsAsync(Guid orderId, Guid patientId);


        // =========================================================================
        // --- Admin Operations (Read-Only Observer) ---
        // =========================================================================
        //Task<Pagination<OrderDto>> GetAllPlatformOrdersAsync(OrderQueryParams queryParams);
        //Task<OrderDetailsDto> GetAdminOrderDetailsAsync(Guid orderId);
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