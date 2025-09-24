using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models; // onde está o ResponseModel

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthCustomController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthCustomController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("validar-senha")]
        public IActionResult ValidarSenha([FromBody] string senha)
        {
            var response = new ResponseModel<string>();

            // 🔹 Recupera a senha do appsettings.json
            var senhaConfig = _configuration["Security:Senha"];

            if (string.IsNullOrEmpty(senhaConfig))
            {
                response.Status = false;
                response.Mensagem = "Configuração de senha não encontrada!";
                return StatusCode(500, response);
            }

            if (senha == senhaConfig)
            {
                response.Status = true;
                response.Mensagem = "Senha válida!";
                response.Dados = "OK";
                return Ok(response);
            }

            response.Status = false;
            response.Mensagem = "Senha incorreta!";
            return Unauthorized(response);
        }
    }
}
