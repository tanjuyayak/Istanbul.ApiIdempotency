namespace Istanbul.ApiIdempotency
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ApiIdempotencyAttribute : Attribute
    {
        private int _timeToLiveInMs;
        private bool _shouldHandleResponseHeaders;

        public ApiIdempotencyAttribute(int timeToLiveInMs, bool shouldHandleResponseHeaders = true)
        {
            _timeToLiveInMs = timeToLiveInMs;
            _shouldHandleResponseHeaders = shouldHandleResponseHeaders;
        }

        public int GetTimeToLiveInMs() { return _timeToLiveInMs; }

        public bool ShouldHandleResponseHeaders() { return _shouldHandleResponseHeaders; }
    }
}