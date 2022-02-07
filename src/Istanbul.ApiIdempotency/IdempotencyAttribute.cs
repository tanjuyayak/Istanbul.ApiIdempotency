namespace Istanbul.ApiIdempotency
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class IdempotencyAttribute : Attribute
    {
        private int _timeToLive;
        private bool _shouldHandleResponseHeaders;

        public IdempotencyAttribute(int timeToLive, bool shouldHandleResponseHeaders = true)
        {
            _timeToLive = timeToLive;
            _shouldHandleResponseHeaders = shouldHandleResponseHeaders;
        }

        public int GetTimeToLive() { return _timeToLive; }

        public bool ShouldHandleResponseHeaders { get { return _shouldHandleResponseHeaders; } }
    }
}