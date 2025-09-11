using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApplication1.Dto;
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

    private async Task<bool> ValidarRecaptchaAsync(string token)
    {
        var secret = "6Ld9g8YrAAAAAI0H8C4aIhC-H_V4kn57kJAJLxuU"; // ⚠️ Substitua pela sua chave secreta do Google
        using var client = new HttpClient();

        var response = await client.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
            null
        );

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var success = root.GetProperty("success").GetBoolean();

        // opcional: também validar score
        if (root.TryGetProperty("score", out var scoreElement))
        {
            var score = scoreElement.GetDouble();
            if (score < 0.5) return false; // rejeita se score < 0.5
        }

        return success;
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


    // GET: api/Atendimento/pendentes/{atendenteId}
    [HttpGet("pendentes/{inspetoriaId}")]
    public async Task<IActionResult> BuscarPendentesPorInspetoria(int inspetoriaId)
    {
        var result = await _atendimentoService.BuscarAtendimentosPendentesPorInspetoria(inspetoriaId);

        if (!result.Status)
            return NotFound(result);

        return Ok(result);
    }


    // 🔹 GET /api/Atendimento/feedbacks
    [HttpGet("feedbacks")]
    [NonAction]
    [Obsolete("Esse endpoint não foi implementado.")]
    public async Task<IActionResult> BuscarAtendimentosFeedbacks()
    {
        var result = await _atendimentoService.BuscarAtendimentosFeedbacks();

        if (!result.Status) return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [NonAction]
    [Obsolete("Esse endpoint não foi implementado.")]
    public async Task<IActionResult> BuscarAtendimentoPorId(int id)
    {
        var result = await _atendimentoService.BuscarAtendimentoPorId(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarNovoAtendimento([FromBody] AtendimentoCaptchaDto atendimentoDto)
    {
        if (atendimentoDto == null)
            return BadRequest(new { Status = false, Mensagem = "Atendimento inválido!" });

        if (string.IsNullOrEmpty(atendimentoDto.RecaptchaToken))
            return BadRequest(new { Status = false, Mensagem = "Token reCAPTCHA ausente!" });

        // 🔹 Valida token com Google
        var valido = await ValidarRecaptchaAsync(atendimentoDto.RecaptchaToken);
        if (!valido)
            return Unauthorized(new { Status = false, Mensagem = "Falha na verificação reCAPTCHA!" });

        // 🔹 Mapeia DTO -> Model
        var atendimento = new Atendimento
        {
            Id = atendimentoDto.Id,
            Motivo = atendimentoDto.Motivo,
            Protocolo = atendimentoDto.Protocolo,
            Data_Atendimento = atendimentoDto.Data_Atendimento,
            Inspetoria_Id = atendimentoDto.Inspetoria_Id,
            Atendente_Id = atendimentoDto.Atendente_Id,
            Profissional_Id = atendimentoDto.Profissional_Id
        };

        var result = await _atendimentoService.CadastrarNovoAtendimento(atendimento);
        if (!result.Status) return BadRequest(result);

        return Ok(result);
    }


    // PUT: api/atendimento/{id} (Id na URL, body com objeto completo)
    [HttpPut("{id}")]
    [NonAction]
    [Obsolete("Esse endpoint não foi implementado.")]
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
    [NonAction]
    [Obsolete("Esse endpoint não foi implementado.")]
    public async Task<IActionResult> RemoverAtendimento(int id)
    {
        var result = await _atendimentoService.RemoverAtendimento(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }
}
