using WebApplication1.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using AutoMapper;
using WebApplication1.Services.Interfaces;
using System.Text.Json;

namespace WebApplication1.Services
{
public class AtendenteService : IAtendenteInterface
    {
        private readonly IConfiguration _configuration;

        public AtendenteService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel<List<AtendenteDto>>> BuscarAtendentes()
        {
            var response = new ResponseModel<List<AtendenteDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = "SELECT at.Id, at.Nome, at.Email, i.nome as Inspetoria,at.Inspetoria_Id, at.Role from Atendente at INNER JOIN Inspetoria i on Inspetoria_Id = i.Id";
                var result = await connection.QueryAsync<AtendenteDto>(sql);

                response.Status = result.Any();
                response.Mensagem = result.Any() ? "Atendentes listados com sucesso!" : "Nenhum atendente encontrado!";
                response.Dados = result.ToList();
            }

            return response;
        }



        public async Task<ResponseModel<AtendenteDto>> BuscarAtendentePorId(int id)
        {
            var response = new ResponseModel<AtendenteDto>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = "SELECT at.Id, at.Nome, at.Email, i.nome as Inspetoria,at.Inspetoria_Id, at.Role from Atendente at INNER JOIN Inspetoria i on Inspetoria_Id = i.Id";
                var result = await connection.QueryFirstOrDefaultAsync<AtendenteDto>(sql, new { Id = id });

                if (result == null)
                {
                    response.Status = false;
                    response.Mensagem = "Atendente não encontrado!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendente encontrado!";
                response.Dados = result;
            }

            return response;
        }

        public async Task<ResponseModel<List<AtendenteDto>>> CriarNovoAtendente(Atendente atendente)
        {
            var response = new ResponseModel<List<AtendenteDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // 1. Verificar se já existe pelo email
                var atendenteExistente = await connection.QueryFirstOrDefaultAsync<AtendenteDto>(
                    "SELECT at.Id, at.Nome, at.Email, i.nome as Inspetoria,at.Inspetoria_Id, at.Role from Atendente at LEFT JOIN Inspetoria i on Inspetoria_Id = i.Id WHERE Email = @Email",
                    new { atendente.Email });

                if (atendenteExistente != null)
                {
                    response.Status = true;
                    response.Mensagem = "Atendente já existente!";
                    response.Dados = new List<AtendenteDto> { atendenteExistente };
                    return response;
                }

                // 2. Se não existe, insere
                var sql = @"
                            INSERT INTO Atendente (Nome, Email, Inspetoria_id, Role)
                            VALUES (@Nome, @Email, @Inspetoria_Id, @Role);
                            SELECT * FROM Atendente WHERE Id = SCOPE_IDENTITY();
                        ";

                var result = await connection.QueryAsync<AtendenteDto>(sql, atendente);

                response.Status = true;
                response.Mensagem = "Atendente cadastrado com sucesso!";
                response.Dados = result.ToList();
            }

            return response;
        }


        public async Task<ResponseModel<List<Atendente>>> EditarAtendente(Atendente atendente)
        {
            var response = new ResponseModel<List<Atendente>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                UPDATE Atendente
                SET Nome = @Nome, Email = @Email, Inspetoria_Id = @Inspetoria_Id, Role = @Role
                WHERE Id = @Id;
            ";

                var linhasAfetadas = await connection.ExecuteAsync(sql, atendente);

                if (linhasAfetadas == 0)
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendente foi atualizado!";
                    return response;
                }

                var atualizado = await connection.QueryAsync<Atendente>(
                    "SELECT * FROM Atendente WHERE Id = @Id", new { atendente.Id });

                response.Status = true;
                response.Mensagem = "Atendente atualizado com sucesso!";
                response.Dados = atualizado.ToList();
            }

            return response;
        }

        public async Task<ResponseModel<List<Atendente>>> AtualizarAtendente(int id, Dictionary<string, object> campos)
        {
            var response = new ResponseModel<List<Atendente>>();

            if (campos == null || campos.Count == 0)
            {
                response.Status = false;
                response.Mensagem = "Nenhum campo enviado para atualizar!";
                return response;
            }

            // Converte JsonElement para valores nativos se necessário
            var camposConvertidos = campos.ToDictionary(
                kvp => kvp.Key,
                kvp =>
                {
                    if (kvp.Value is JsonElement jsonElement)
                    {
                        return jsonElement.ValueKind switch
                        {
                            JsonValueKind.String => (object)jsonElement.GetString(),
                            JsonValueKind.Number => jsonElement.TryGetInt32(out int i) ? (object)i : jsonElement.GetDouble(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Null => null,
                            _ => kvp.Value
                        };
                    }
                    return kvp.Value;
                });

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var sets = string.Join(", ", camposConvertidos.Keys.Select(k => $"{k} = @{k}"));
                var sql = $"UPDATE Atendente SET {sets} WHERE Id = @Id";

                camposConvertidos.Add("Id", id);

                var linhasAfetadas = await connection.ExecuteAsync(sql, camposConvertidos);

                if (linhasAfetadas == 0)
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendente foi atualizado!";
                    return response;
                }

                var atualizado = await connection.QueryAsync<Atendente>(
                    "SELECT * FROM Atendente WHERE Id = @Id", new { Id = id });

                response.Status = true;
                response.Mensagem = "Atendente atualizado com sucesso!";
                response.Dados = atualizado.ToList();
            }

            return response;
        }

        public async Task<ResponseModel<Atendente>> RemoverAtendente(int id)
        {
            var response = new ResponseModel<Atendente>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var atendente = await connection.QueryFirstOrDefaultAsync<Atendente>(
                    "SELECT * FROM Atendente WHERE Id = @Id", new { Id = id });

                if (atendente == null)
                {
                    response.Status = false;
                    response.Mensagem = "Atendente não encontrado!";
                    return response;
                }

                var sql = "DELETE FROM Atendente WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });

                response.Status = true;
                response.Mensagem = "Atendente removido com sucesso!";
                response.Dados = atendente;
            }

            return response;
        }
    }

}

