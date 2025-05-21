using EmployeeHub.Common.ApiResponse;
using EmployeeHub.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;


namespace TheBookClub.Middleware.ExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Default to 500 Internal Server Error
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred { " + exception.Message + " }";

            if (exception is ApiException apiException)
            {
                // Use the status code and message from the custom exception
                statusCode = apiException.StatusCode;
                message = apiException.Message;
            }

            context.Response.StatusCode = statusCode;

            var response = new ApiResponse
            {
                StatusCode = statusCode,
                Message = message,
                Data = null
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}