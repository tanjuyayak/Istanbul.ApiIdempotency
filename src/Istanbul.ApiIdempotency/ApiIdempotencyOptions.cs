using Istanbul.ApiIdempotency.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istanbul.ApiIdempotency
{
    public class ApiIdempotencyOptions
    {
        public IApiIdempotencyDataStoreProvider IdempotencyDataStoreProvider { get; init; }
        public string IdempotencyHeaderKey { get; init; }
    }
}
