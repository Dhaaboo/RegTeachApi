using RegTeachApi.Responses;
using System.Net;
using System.Text.Json;

namespace RegTeachApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType =
                    "application/json";

                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                var response =
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message =
                            "An unexpected error occurred.",

                        Errors = new()
                        {
                        ex.Message
                        }
                    };

                var json =
                    JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(
                    json);
            }
        }
    }
}
