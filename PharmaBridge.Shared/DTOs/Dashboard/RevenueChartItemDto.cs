using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Dashboard
{
    public class RevenueChartItemDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
