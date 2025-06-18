using Microsoft.AspNetCore.Mvc;

namespace Soccer_IQ.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("✅ API is healthy and running!");
        }
    }
}
