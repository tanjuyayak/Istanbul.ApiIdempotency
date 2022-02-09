namespace Istanbul.ApiIdempotency.Core
{
    public interface IApiIdempotencyDataStoreProvider
    {
        Task<ApiIdempotencyResult> TryAcquireIdempotencyAsync(string key, int timeToLiveInMs);
        Task SetDataAsync(string jsonResponseData, Dictionary<string, string> responseHeaders);
    }
}