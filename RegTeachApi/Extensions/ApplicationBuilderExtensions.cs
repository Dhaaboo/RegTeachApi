using RegTeachApi.Middleware;

namespace RegTeachApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder
            UseGlobalExceptionMiddleware(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware
                <GlobalExceptionMiddleware>();
        }
    }
}
