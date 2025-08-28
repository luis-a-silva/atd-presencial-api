using Dapper;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;


public class InspetoriaService : IInspetoriaInterface
{
    private readonly IConfiguration _configuration;

    public InspetoriaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Listar todas as inspetorias
    public async Task<ResponseModel<List<Inspetoria>>> BuscarInspetoria()
    {
        var response = new ResponseModel<List<Inspetoria>>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            var sql = "SELECT * FROM Inspetoria";
            var result = await connection.QueryAsync<Inspetoria>(sql);

            response.Status = result.Any();
            response.Mensagem = result.Any() ? "Inspetorias listadas com sucesso!" : "Nenhuma inspetoria encontrada!";
            response.Dados = result.ToList();
        }

        return response;
    }

    // Buscar inspetoria por ID
    public async Task<ResponseModel<Inspetoria>> BuscarInspetoriaPorId(int id)
    {
        var response = new ResponseModel<Inspetoria>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            var sql = "SELECT * FROM Inspetoria WHERE Id = @Id";
            var result = await connection.QueryFirstOrDefaultAsync<Inspetoria>(sql, new { Id = id });

            if (result == null)
            {
                response.Status = false;
                response.Mensagem = "Inspetoria não encontrada!";
                return response;
            }

            response.Status = true;
            response.Mensagem = "Inspetoria encontrada!";
            response.Dados = result;
        }

        return response;
    }

    // Cadastrar nova inspetoria
    public async Task<ResponseModel<List<Inspetoria>>> CadastrarNovaInspetoria(Inspetoria inspetoria)
    {
        var response = new ResponseModel<List<Inspetoria>>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            var sql = @"
                INSERT INTO Inspetoria (Nome)
                VALUES (@Nome);

                SELECT * FROM Inspetoria WHERE Id = SCOPE_IDENTITY();
            ";

            var result = await connection.QueryAsync<Inspetoria>(sql, inspetoria);

            response.Status = true;
            response.Mensagem = "Inspetoria cadastrada com sucesso!";
            response.Dados = result.ToList();
        }

        return response;
    }

    // Editar inspetoria completa
    public async Task<ResponseModel<List<Inspetoria>>> EditarInspetorias(Inspetoria inspetoria)
    {
        var response = new ResponseModel<List<Inspetoria>>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            var sql = @"
                UPDATE Inspetoria
                SET Nome = @Nome
                WHERE Id = @Id
            ";

            var linhasAfetadas = await connection.ExecuteAsync(sql, inspetoria);

            if (linhasAfetadas == 0)
            {
                response.Status = false;
                response.Mensagem = "Nenhuma inspetoria foi atualizada!";
                return response;
            }

            var atualizado = await connection.QueryAsync<Inspetoria>(
                "SELECT * FROM Inspetoria WHERE Id = @Id", new { inspetoria.Id });

            response.Status = true;
            response.Mensagem = "Inspetoria atualizada com sucesso!";
            response.Dados = atualizado.ToList();
        }

        return response;
    }

    // Remover inspetoria
    public async Task<ResponseModel<Inspetoria>> RemoverInspetoria(int id)
    {
        var response = new ResponseModel<Inspetoria>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            var inspetoria = await connection.QueryFirstOrDefaultAsync<Inspetoria>(
                "SELECT * FROM Inspetoria WHERE Id = @Id", new { Id = id });

            if (inspetoria == null)
            {
                response.Status = false;
                response.Mensagem = "Inspetoria não encontrada!";
                return response;
            }

            var sql = "DELETE FROM Inspetoria WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });

            response.Status = true;
            response.Mensagem = "Inspetoria removida com sucesso!";
            response.Dados = inspetoria;
        }

        return response;
    }
}
