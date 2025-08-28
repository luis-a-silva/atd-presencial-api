using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces; // supondo que ProfissionalService esteja aqui

[ApiController]
[Route("api/[controller]")]
public class ProfissionalController : ControllerBase
{
    private readonly IProfissionalInterface _profissionalService;

    public ProfissionalController(IProfissionalInterface profissionalService)
    {
        _profissionalService = profissionalService;
    }

    /// <summary>
    /// Cadastra um novo profissional
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CadastrarProfissional([FromBody] Profissional profissional)
    {
        if (profissional == null)
            return BadRequest(new { Status = false, Mensagem = "Profissional inválido!" });

        var result = await _profissionalService.CadastrarProfissional(profissional);

        if (!result.Status)
            return BadRequest(result);

        return Ok(result);
    }
}
