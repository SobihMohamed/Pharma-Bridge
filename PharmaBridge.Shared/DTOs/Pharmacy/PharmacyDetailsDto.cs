using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyDetailsDto
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; }
        public string? TextAddress { get; set; }
        public decimal AverageRating { get; set; }
        public bool Is24Hours { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public string? ContactPhone { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
