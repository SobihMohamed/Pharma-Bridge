
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Order;

namespace PharmaBridge.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [HttpGet("pharmacy/{pharmacyId}")]
        public async Task<IActionResult> GetPharmacyOrders(int pharmacyId, [FromQuery] OrderQueryParams queryParams)
        {
            var result = await orderService.GetPharmacyOrdersAsync(pharmacyId, queryParams);
            return Ok(result);
        }

        [HttpGet("{orderId}/pharmacy/{pharmacyId}")]
        public async Task<IActionResult> GetPharmacyOrderDetails(int orderId, int pharmacyId)
        {
            var result = await orderService.GetPharmacyOrderDetailsAsync(orderId, pharmacyId);
            return Ok(result);
        }

        [HttpPatch("{orderId}/status/pharmacy/{pharmacyId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, int pharmacyId, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await orderService.UpdateOrderStatusAsync(orderId, dto, pharmacyId);
            return Ok(result);
        }

        [HttpPost("create-from-bid/{bidId}")]
        public async Task<IActionResult> CreateFromBid(int bidId)
        {
            var result = await orderService.CreateOrderFromBidAsync(bidId);
            return Ok(result);
        }
    }
}