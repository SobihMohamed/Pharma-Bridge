using PharmaBridge.Shared.DTOs.PrescriptionRequestHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests.AdminReq
{
    public class AdminPrescriptionRequestDetailsDto : AdminPrescriptionRequestDto
    {
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string FullAddress { get; set; } = string.Empty; // العنوان بالتفصيل

        // العروض اللي اتقدمت بالتفصيل الممل
        public List<AdminBidDto> Bids { get; set; } = new();

        // الهيستوري بتاع الطلب
        public List<PrescriptionRequestHistoryDto> History { get; set; } = new();
    }
}
