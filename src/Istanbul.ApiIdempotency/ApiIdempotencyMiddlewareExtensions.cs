using Microsoft.AspNetCore.Builder;

namespace Istanbul.ApiIdempotency
{
    public static class ApiIdempotencyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiIdempotency(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiIdempotencyMiddleware>();
        }
    }
}