using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApplication1.Controllers


{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;


        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }


        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            _logger.LogInformation($"Token validado para usuário: {User.Identity?.Name}");

            return Ok(new
            {
                valid = true,
                user = User.Identity?.Name,
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            });
        }


        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            var user = new
            {
                Name = User.FindFirst("name")?.Value
                    ?? User.FindFirst("preferred_username")?.Value
                    ?? User.Identity?.Name,
                Email = User.FindFirst("email")?.Value
                    ?? User.FindFirst("preferred_username")?.Value,
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value,
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Claims = claims // Para debug - remova em produção
            };

            _logger.LogInformation($"Informações do usuário solicitadas: {user.Name}");

            return Ok(user);
        }


        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}