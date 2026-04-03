using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Pagination
{
    public class PaginationResponse<TData>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; } // total items in databsse that match the query you have been sent 
        public IReadOnlyList<TData> Data { get; set; }
        public PaginationResponse(int index , int size , int total , IReadOnlyList<TData> data)
        {
            PageIndex = index;
            PageSize = size;
            TotalCount = total;
            Data = data;
        }
    }
}
