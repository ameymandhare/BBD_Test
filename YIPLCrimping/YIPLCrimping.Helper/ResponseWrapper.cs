using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace YIPLCrimping.Helper
{
    public class ApiResponseWrapper<T>
    {
        public T Result { get; set; }
        public bool HasError { get; set; }
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        public ApiResponseWrapper(T result, bool hasError = false, int statusCode = 200, bool success = true, string? message = null)
        {
            Result = result;
            HasError = hasError;
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Message = message;
        }

        public async Task<IActionResult> FinalizeExceptionAsync(Exception ex)
        {
            var responseData = new JObject
        {
            { "HasError", true },
            { "Message", ex.Message },
            //{ "Details", ex.StackTrace },  // You might want to skip this in production
            { "StatusCode", 500 },
            { "Success", false }
        };

            return await Task.FromResult(new ObjectResult(responseData)
            {
                StatusCode = 500
            });
        }
    }

    public static class ErrorResponseWrapper
    {
        public static JObject CreateErrorResponse(string message, int statusCode, int errorNumber)
        {
            return new JObject
            {
                { "Message", message },
                { "HasError", true },
                { "StatusCode", statusCode },
                { "Success", false },
                { "ErrorNumber", errorNumber }
            };
        }
    }
}