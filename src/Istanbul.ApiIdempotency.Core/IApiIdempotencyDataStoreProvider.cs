namespace Istanbul.ApiIdempotency.Core
{
    public interface IApiIdempotencyDataStoreProvider
    {
        Task<ApiIdempotencyResult> TryCheckKeyExistsAsync(string key, int timeToLiveInSec);
        Task SetDataAsync(string key, int timeToLiveInSec, string responseBody, int httpStatusCode, Dictionary<string, string> responseHeaders);
    }
}