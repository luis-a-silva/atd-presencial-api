using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AtendenteController : ControllerBase
{
    private readonly IAtendenteInterface _atendenteService;

    public AtendenteController(IAtendenteInterface atendenteService)
    {
        _atendenteService = atendenteService;
    }

    [HttpGet]
    public async Task<IActionResult> BuscarAtendentes()
    {
        var result = await _atendenteService.BuscarAtendentes();
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarAtendentePorId(int id)
    {
        var result = await _atendenteService.BuscarAtendentePorId(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CriarNovoAtendente([FromBody] Atendente atendente)
    {
        if (atendente == null)
            return BadRequest(new { Status = false, Mensagem = "Atendente inválido!" });

        var result = await _atendenteService.CriarNovoAtendente(atendente);

        if (!result.Status)
            return BadRequest(result);

        return Ok(result);
    }


    // PUT: api/atendente/{id} (Id na URL, body só com campos a atualizar)
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarAtendente(int id, [FromBody] Atendente atendente)
    {
        if (atendente == null)
            return BadRequest(new { Status = false, Mensagem = "Atendente inválido!" });

        // garante que o Id do objeto seja o da URL
        atendente.Id = id;

        var result = await _atendenteService.EditarAtendente(atendente);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    // PATCH: api/atendente/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> AtualizarAtendente(int id, [FromBody] Dictionary<string, object> campos)
    {
        if (campos == null || campos.Count == 0)
            return BadRequest(new { Status = false, Mensagem = "Nenhum campo enviado para atualizar!" });

        var result = await _atendenteService.AtualizarAtendente(id, campos);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverAtendente(int id)
    {
        var result = await _atendenteService.RemoverAtendente(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }
}
