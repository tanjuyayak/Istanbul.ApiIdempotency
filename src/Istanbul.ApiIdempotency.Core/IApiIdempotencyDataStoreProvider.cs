namespace Istanbul.ApiIdempotency.Core
{
    public interface IApiIdempotencyDataStoreProvider
    {
        Task<ApiIdempotencyResult> TryAcquireIdempotencyAsync(string key, int timeToLiveInMs);
        Task SetDataAsync(string key, string responseBody, int httpStatusCode, Dictionary<string, string> responseHeaders);
    }
}