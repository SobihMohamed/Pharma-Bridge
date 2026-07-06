using PharmaBridge.Shared.DTOs.Order;
using PharmaBridge.Shared.DTOs.PharmaRequests;

namespace PharmaBridge.Shared.DTOs.Patient
{
    public class PatientHomeDto
    {
        public IReadOnlyList<PrescriptionRequestDto> LatestRequests { get; set; } = [];

        public IReadOnlyList<OrderDto> recentOrders { get; set; } = [];
    }
}