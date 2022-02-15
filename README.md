##  About Istanbul.ApiIdempotency 

###  What is Idempotency? 
Idempotency is a term given to certain operations in mathematics and computer science. In mathematics, it can be seen when we multiply any number  by 1. It doesn’t matter how many times we perform this operation the result will be always same.

It is also very important concept in computer science. Idempotency concept is allowing you to retry a request multiple times while only performing the action once. Let's imagine that you have an api which is responsible for accepting payments for a specific order. From client application perspective there are many situations (timeouts, network issues, application failures, bugs and even clicking "complete" button more than one time) which might require safely retrying the same operation with the guarantee that the operation will only be executed once.  


## Getting Started

Istanbul.ApiIdempotency is a .net package which focuses on helping developers to implement this concept easily to their applications. Package is supporting Redis as data store at the moment but it also allows you to extend it and work  with some other data stores. Implementation for other data stores is in my roadmap.

## Installation

You can install [Istanbul.ApiIdempotency with NuGet](https://www.nuget.org/packages/Istanbul.Idempotency):

    Install-Package Istanbul.ApiIdempotency
    Install-Package Istanbul.ApiIdempotency.Redis.StackExchange
    
Or via the .NET Core command line interface:

    dotnet add package Istanbul.ApiIdempotency
    dotnet add package Istanbul.ApiIdempotency.Redis.StackExchange


## Usage

Package is designed to work with different data stores and it supports Redis at the moment. There are different Redis c# driver to connect and use Redis so I decided to provide different packages idempotency data store packages for different Redis drivers. First version of Redis data store for this package supports **StackExchange** driver so that's why **RedisApiIdempotencyDataStoreProvider** accepts **IConnectionMultiplexer** as constructor parameter. You are free to configure **IConnectionMultiplexer** as you wish.


Configure settings:
``` csharp
var redisConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

services.AddApiIdempotency(options =>
{
    options.IdempotencyHeaderKey = "X-Idempotency-Key";
    options.IdempotencyDataStoreProvider = new RedisApiIdempotencyDataStoreProvider(redisConnectionMultiplexer);
});
```
Use idempotency:
``` csharp
app.UseApiIdempotency();
```

Activate idempotency with 20 seconds of timeout:
``` csharp
[HttpPost]
[Route("api/[controller]/create")]
[ApiIdempotency(20)]
public IActionResult Create()
{
    return Ok(new { OrderCreationDate = DateTime.Now });
}
```

## Roadmap

- [x] Creating basic idempotency package
- [x] Nuget availability
- [ ] Auto generated idempotency keys
- [ ] Allowing project to provide key (not only from request header)
- [ ] Supporting more data stores
    - [ ] Couchbase
    - [ ] Memcached

See the [open issues](https://github.com/tanjuyayak/Istanbul.ApiIdempotency/issues) for a full list of proposed features (and known issues).


## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue.
Don't forget to give the project a star :smile:

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.