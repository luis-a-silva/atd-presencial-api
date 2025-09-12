using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardInterface _dashboardService;

        public DashboardController(IDashboardInterface dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // 🔹 GET /api/Dashboard/total
        [HttpGet("total")]
        public async Task<IActionResult> Total()
        {
            var result = await _dashboardService.TotalAsync();

            if (!result.Status) return NotFound(result);

            return Ok(result);
        }

        // 🔹 GET /api/Dashboard/por-inspetoria
        [HttpGet("por-inspetoria")]
        public async Task<IActionResult> PorInspetoria()
        {
            var result = await _dashboardService.PorInspetoriaAsync();

            if (!result.Status) return NotFound(result);

            return Ok(result);
        }

        // 🔹 GET /api/Dashboard/por-motivo
        [HttpGet("por-motivo")]
        public async Task<IActionResult> PorMotivo()
        {
            var result = await _dashboardService.PorMotivoAsync();

            if (!result.Status) return NotFound(result);

            return Ok(result);
        }

        // 🔹 GET /api/Dashboard/por-tipo
        [HttpGet("por-tipo")]
        public async Task<IActionResult> PorTipo()
        {
            var result = await _dashboardService.PorTipoAsync();

            if (!result.Status) return NotFound(result);

            return Ok(result);
        }

        // 🔹 GET /api/Dashboard/serie-mensal?anoInicio=2024&anoFim=2025
        [HttpGet("serie-mensal")]
        public async Task<IActionResult> SerieMensal([FromQuery] int anoInicio, [FromQuery] int anoFim)
        {
            var result = await _dashboardService.SerieMensalAsync(anoInicio, anoFim);

            if (!result.Status) return NotFound(result);

            return Ok(result);
        }
    }
}

