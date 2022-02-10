namespace Istanbul.ApiIdempotency.Core
{
    public class ApiIdempotencyResult
    {
        public bool IsIdempotencyAlreadyAcquired { get; init; }
        public ApiResponseCache ResponseCache { get; init; }
    }
}
