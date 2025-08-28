using Dapper;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class ProfissionalService : IProfissionalInterface
    {
        private readonly IConfiguration _configuration;

        public ProfissionalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel<List<Profissional>>> CadastrarProfissional(Profissional profissional)
        {
            var response = new ResponseModel<List<Profissional>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // 1. Verificar se já existe
                var profissionalExistente = await connection.QueryFirstOrDefaultAsync<Profissional>(
                    "SELECT * FROM Profissional WHERE Num_Cadastro_Nacional = @NumeroCadastroNacional",
                    new { profissional.NumeroCadastroNacional });

                if (profissionalExistente != null)
                {
                    response.Status = true;
                    response.Mensagem = "Profissional já existente!";
                    response.Dados = new List<Profissional> { profissionalExistente };
                    return response;
                }

                // 2. Se não existe, insere
                var sql = @"
                    INSERT INTO Profissional (Tipo_Id, Num_Cadastro_Nacional)
                    VALUES (@Tipo, @NumeroCadastroNacional);

                    SELECT * FROM Profissional WHERE Id = SCOPE_IDENTITY();
                ";

                var result = await connection.QueryAsync<Profissional>(sql, profissional);

                response.Status = true;
                response.Mensagem = "Profissional cadastrado com sucesso!";
                response.Dados = result.ToList();
            }

            return response;
        }
    }
}
