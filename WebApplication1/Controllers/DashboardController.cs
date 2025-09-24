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

        [HttpGet("total")]
        public async Task<IActionResult> Total([FromQuery] int? inspetoriaId)
            => Ok(await _dashboardService.TotalAsync(inspetoriaId));

        [HttpGet("por-inspetoria")]
        public async Task<IActionResult> PorInspetoria([FromQuery] int? inspetoriaId)
            => Ok(await _dashboardService.PorInspetoriaAsync(inspetoriaId));

        [HttpGet("por-motivo")]
        public async Task<IActionResult> PorMotivo([FromQuery] int? inspetoriaId)
            => Ok(await _dashboardService.PorMotivoAsync(inspetoriaId));

        [HttpGet("por-tipo")]
        public async Task<IActionResult> PorTipo([FromQuery] int? inspetoriaId)
            => Ok(await _dashboardService.PorTipoAsync(inspetoriaId));

        [HttpGet("serie-mensal")]
        public async Task<IActionResult> SerieMensal([FromQuery] int anoInicio, [FromQuery] int anoFim, [FromQuery] int? inspetoriaId)
            => Ok(await _dashboardService.SerieMensalAsync(anoInicio, anoFim, inspetoriaId));
    }

}
