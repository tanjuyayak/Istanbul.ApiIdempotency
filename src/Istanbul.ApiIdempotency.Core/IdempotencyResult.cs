namespace Istanbul.ApiIdempotency.Core
{
    public class IdempotencyResult
    {
        public bool IsIdempotencyAlreadyAcquired { get; init; }
        public string Data { get; init; }
    }
}
