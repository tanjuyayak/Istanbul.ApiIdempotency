﻿using Istanbul.ApiIdempotency.Core;
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
            if (apiIdempotencyAttribute == null)
            {
                await _next(context);
                return;
            }

            var timeToLiveInMs = apiIdempotencyAttribute.GetTimeToLiveInMs();
            if (timeToLiveInMs <= 0)
            {
                throw new ArgumentException($"TTL for idempotency can't be zero or lower than zero!");
            }

            var idempotencyHeaderKey = options.Value.IdempotencyHeaderKey;
            StringValues idempotencyKeyValue;
            if (!context.Request.Headers.TryGetValue(idempotencyHeaderKey, out idempotencyKeyValue))
            {
                throw new ArgumentException($"Request header doesn't contain '{idempotencyHeaderKey}' key for idempotency!");
            }

            if (string.IsNullOrWhiteSpace(idempotencyKeyValue))
            {
                throw new ArgumentException($"Idempotency header key '{idempotencyHeaderKey}' is null or string empty or contains only white space");
            }

            var result = await apiIdempotencyDataStoreProvider.TryAcquireIdempotencyAsync(idempotencyKeyValue, timeToLiveInMs);
            if (result.IsIdempotencyAlreadyAcquired)
            {
                context.Response.StatusCode = result.HttpStatusCode;

                if (result.ResponseHeaders != null && result.ResponseHeaders.Count > 0)
                {
                    foreach (var responseHeader in result.ResponseHeaders)
                    {
                        context.Response.Headers[responseHeader.Key] = responseHeader.Value;
                    }
                }

                await context.Response.WriteAsync(result.ResponseBody);

                return;
            }

            var originalResponseBody = context.Response.Body;

            try
            {
                using (var responseMemoryStream = new MemoryStream())
                {
                    context.Response.Body = responseMemoryStream;

                    await _next(context);

                    responseMemoryStream.Position = 0;

                    var responseBody = new StreamReader(responseMemoryStream).ReadToEnd();
                    var responseHeaders = GetResponseHeaders(context);

                    await apiIdempotencyDataStoreProvider.SetDataAsync(idempotencyKeyValue, responseBody, context.Response.StatusCode, responseHeaders);

                    responseMemoryStream.Position = 0;

                    await responseMemoryStream.CopyToAsync(originalResponseBody);
                }
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }
        }

        private static Dictionary<string, string> GetResponseHeaders(HttpContext context)
        {
            return context.Response.Headers.ToDictionary(a => a.Key, a => (string)a.Value);
        }
    }
}
