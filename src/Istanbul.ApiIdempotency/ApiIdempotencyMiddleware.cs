using Istanbul.ApiIdempotency.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Istanbul.ApiIdempotency
{
    public class ApiIdempotencyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiIdempotencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiIdempotencyDataStoreProvider apiIdempotencyDataStoreProvider, IOptions<ApiIdempotencyInternalOptions> options)
        {
            var apiIdempotencyAttribute = context.GetEndpoint()?.Metadata?.GetMetadata<ApiIdempotencyAttribute>();
            if (apiIdempotencyAttribute != null)
            {
                var timeToLiveInMs = apiIdempotencyAttribute.GetTimeToLiveInMs();
                if (timeToLiveInMs <= 0)
                {
                    throw new ArgumentException($"TTL for idempotency can't be zero or lower than zero!");
                }

                var shouldHandleResponseHeaders = apiIdempotencyAttribute.ShouldHandleResponseHeaders();

                var idempotencyHeaderKey = options.Value.IdempotencyHeaderKey;
                StringValues idempotencyKeyValue;
                if (!context.Request.Headers.TryGetValue(idempotencyHeaderKey, out idempotencyKeyValue))
                {
                    throw new ArgumentException($"Request header doesn't contain '{idempotencyHeaderKey}' key for idempotency!");
                }

                var result = await apiIdempotencyDataStoreProvider.TryAcquireIdempotencyAsync(idempotencyKeyValue, timeToLiveInMs);
                if (result.IsIdempotencyAlreadyAcquired)
                {

                }
                else
                {
                    var originalResponseBody = context.Response.Body;

                    try
                    {
                        using (var responseMemoryStream = new MemoryStream())
                        {
                            context.Response.Body = responseMemoryStream;

                            await _next(context);

                            responseMemoryStream.Position = 0;

                            string jsonResponseBody = new StreamReader(responseMemoryStream).ReadToEnd();
                            Dictionary<string, string> responseHeaders = null;
                            if (shouldHandleResponseHeaders)
                            {
                                responseHeaders = GetResponseHeaders(context);
                            }

                            await apiIdempotencyDataStoreProvider.SetDataAsync(jsonResponseBody, responseHeaders);

                            responseMemoryStream.Position = 0;

                            await responseMemoryStream.CopyToAsync(originalResponseBody);
                        }
                    }
                    finally
                    {
                        context.Response.Body = originalResponseBody;
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        private Dictionary<string, string> GetResponseHeaders(HttpContext context)
        {
            return new Dictionary<string, string>();
        }
    }
}
