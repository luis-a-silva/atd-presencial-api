using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class InspetoriaController : ControllerBase
{
    private readonly IInspetoriaInterface _inspetoriaService;

    public InspetoriaController(IInspetoriaInterface inspetoriaService)
    {
        _inspetoriaService = inspetoriaService;
    }

    // GET: api/inspetoria
    [HttpGet]
    public async Task<IActionResult> BuscarInspetorias()
    {
        var result = await _inspetoriaService.BuscarInspetoria();
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }

    // GET: api/inspetoria/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarInspetoriaPorId(int id)
    {
        var result = await _inspetoriaService.BuscarInspetoriaPorId(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }

    // POST: api/inspetoria
    [HttpPost]
    public async Task<IActionResult> CadastrarNovaInspetoria([FromBody] Inspetoria inspetoria)
    {
        if (inspetoria == null)
            return BadRequest(new { Status = false, Mensagem = "Inspetoria inválida!" });

        var result = await _inspetoriaService.CadastrarNovaInspetoria(inspetoria);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    // PUT: api/inspetoria/{id} (Id na URL, body só com campos a atualizar)
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarInspetoria(int id, [FromBody] Inspetoria inspetoria)
    {
        if (inspetoria == null)
            return BadRequest(new { Status = false, Mensagem = "Inspetoria inválida!" });

        // garante que o Id do objeto seja o da URL
        inspetoria.Id = id;

        var result = await _inspetoriaService.EditarInspetorias(inspetoria);
        if (!result.Status) return BadRequest(result);
        return Ok(result);
    }

    // DELETE: api/inspetoria/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverInspetoria(int id)
    {
        var result = await _inspetoriaService.RemoverInspetoria(id);
        if (!result.Status) return NotFound(result);
        return Ok(result);
    }
}
