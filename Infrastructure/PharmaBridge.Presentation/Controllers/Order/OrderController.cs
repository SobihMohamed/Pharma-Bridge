using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Order;

namespace PharmaBridge.Presentation.Controllers.Order
{
    [Route("api/orders")]
    [Authorize(Roles = "Admin,PharmacyOwner")]
    public class OrderController(IOrderService orderService) : AppBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int pharmacyId,
            [FromQuery] OrderQueryParams queryParams)
        {
            var result = await orderService.GetPharmacyOrdersAsync(pharmacyId, queryParams);
            return Success(result, "Orders retrieved successfully");
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(
            int orderId,
            [FromQuery] int pharmacyId)
        {
            var result = await orderService.GetPharmacyOrderDetailsAsync(orderId, pharmacyId);
            return Success(result, "Order details retrieved successfully");
        }

        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusDto dto,
            [FromQuery] int pharmacyId)
        {
            var result = await orderService.UpdateOrderStatusAsync(orderId, dto, pharmacyId);
            return Success(result, "Order status updated successfully");
        }

        [AllowAnonymous]
        [HttpPost("create-from-bid/{bidId}")]
        public async Task<IActionResult> CreateFromBid(int bidId)
        {
            var result = await orderService.CreateOrderFromBidAsync(bidId);
            return Created(result, "Order created successfully");
        }
    }
}