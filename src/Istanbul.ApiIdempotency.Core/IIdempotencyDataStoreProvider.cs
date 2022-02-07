namespace Istanbul.ApiIdempotency.Core
{
    public interface IIdempotencyDataStoreProvider
    {
        Task<IdempotencyResult> TryAcquireIdempotencyAsync(string key, int timeToLiveInMs);
        Task SetDataAsync(string data);
    }
}