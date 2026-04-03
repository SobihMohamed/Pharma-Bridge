using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params
{
    public class BaseQueryParam
    {

        private int _pageIndex=1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = (value < 1) ? 1 : value;
        }

        private int _maxPageSize = 20;
        public int PageSize
        {
            get => _maxPageSize;
            set => _maxPageSize = (value > 20) ? 20 : value;
        }

        private string? _search { get; set; }
        public string? Search
        {
            get => _search;
            set => _search = value?.Trim().ToLower();
        }
    }
}
