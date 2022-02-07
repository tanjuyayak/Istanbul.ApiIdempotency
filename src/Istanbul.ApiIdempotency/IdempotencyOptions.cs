using Istanbul.ApiIdempotency.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istanbul.ApiIdempotency
{
    public class IdempotencyOptions
    {
        public IIdempotencyDataStoreProvider IdempotencyDataStoreProvider { get; init; }
    }
}
