using PharmaBridge.Shared.Common.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Pharmacy
{
    public class PharmacyRatingQueryParams : BaseQueryParam
    {
        // Optional: filter by rating value (1-5)
        public int? RatingValue { get; set; }

        // Optional: search in comments
        public new string? Search { get; set; }
    }
}