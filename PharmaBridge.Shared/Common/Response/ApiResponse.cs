using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Response
{
    public class ApiResponse<TData>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; } // pagination response
        public List<string>? Errors { get; set; } = new List<string>();
        //Success Response
        public ApiResponse(TData data , string msg , int statusCode = 200)
        {
            Data = data;
            Message = msg;
            StatusCode = statusCode;
            IsSuccess = true;
        }

        // Failed Response 
        public ApiResponse(string msg, int statusCode = 400, List<string>? errors = null )
        {
            IsSuccess = false;
            Message = msg;
            StatusCode = statusCode;
            Errors = errors;
        }

    }
}
