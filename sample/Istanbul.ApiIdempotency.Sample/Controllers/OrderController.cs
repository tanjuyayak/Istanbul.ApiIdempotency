using Microsoft.AspNetCore.Mvc;

namespace Istanbul.ApiIdempotency.Sample.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpPost]
        [Route("api/[controller]/create")]
        [ApiIdempotency(20)]
        public IActionResult Create()
        {
            return Ok(new { OrderCreationDate = DateTime.Now });
        }
    }
}