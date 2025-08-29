using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AtendimentoController : ControllerBase
{
    private readonly IAtendimentoInterface _atendimentoService;

    public AtendimentoController(IAtendimentoInterface atendimentoService)
    {
        _atendimentoService = atendimentoService;
    }

    // 🔹 GET /api/Atendimento/todos
    [HttpGet("todos")]
    public async Task<IActionResult> BuscarTodosAtendimentos()
    {
        var result = await _atendimentoService.BuscarTodosAtendimentos();

        if (!result.Status) return NotFound(result);

        return Ok(result);
    }

    // 🔹 GET /api/Atendimento/pendentes
    [HttpGet("pendentes")]
    public async Task<IActionResult> BuscarAtendimentosPendentes()
    {
        var result = await _atendimentoService.BuscarAtendimentosPendentes();

        if (!result.Status) return NotFound(result);

        return Ok(result);
    }

    // 🔹 GET /api/Atendimento/feedbacks
    [HttpGet("feedbacks")]
    public async Task<IActionResult> BuscarAtendimentosFeedbacks()
    {
        var result = await _atendimentoService.BuscarAtendimentosFeedbacks();

        if (!result.Status) return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarAtendimentoPorId(int id)
    {
        var result = await _atendimentoService.BuscarAtendimentoPorId(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarNovoAtendimento([FromBody] Atendimento atendimento)
    {
        if (atendimento == null)
            return BadRequest(new { Status = false, Mensagem = "Atendimento inválido!" });

        // Adicione esta validação
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _atendimentoService.CadastrarNovoAtendimento(atendimento);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    // PUT: api/atendimento/{id} (Id na URL, body com objeto completo)
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarAtendimento(int id, [FromBody] Atendimento atendimento)
    {
        if (atendimento == null)
            return BadRequest(new { Status = false, Mensagem = "Atendimento inválido!" });

        atendimento.Id = id;

        var result = await _atendimentoService.EditarAtendimento(atendimento);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    // PATCH: api/atendimento/{id} (atualização parcial)
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> AtualizarAtendimento(int id, [FromBody] Dictionary<string, object> campos)
    {
        if (campos == null || campos.Count == 0)
            return BadRequest(new { Status = false, Mensagem = "Nenhum campo enviado para atualizar!" });

        var result = await _atendimentoService.AtualizarAtendimento(id, campos);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverAtendimento(int id)
    {
        var result = await _atendimentoService.RemoverAtendimento(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }
}
