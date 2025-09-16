using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "listening",
                message = "API está rodando e pronta para receber requisições 🚀",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
