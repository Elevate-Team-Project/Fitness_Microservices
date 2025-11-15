using System;
using System.Collections.Generic;

namespace WorkoutService.Shared
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse(T data, string message = null)
        {
            IsSuccess = true;
            Message = message;
            Data = data;
            Errors = new List<string>();
            StatusCode = 200;
            Timestamp = DateTime.UtcNow;
        }

        public ApiResponse(string message, bool isSuccess = false)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = new List<string>();
            StatusCode = isSuccess ? 200 : 400;
            Timestamp = DateTime.UtcNow;
        }
    }
}