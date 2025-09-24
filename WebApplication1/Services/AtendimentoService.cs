using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using WebApplication1.Dto;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class AtendimentoService : IAtendimentoInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AtendimentoService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        // Buscar todos os atendimentos
        public async Task<ResponseModel<List<AtendimentoDto>>> BuscarTodosAtendimentos()
        {
            var response = new ResponseModel<List<AtendimentoDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
               SELECT 
                atd.Id, 
                atd.Motivo, 
                CAST(atd.Protocolo AS VARCHAR) AS Protocolo,
                p.Num_Cadastro_Nacional AS Profissional, 
                t.Nome AS Tipo, 
                at.Nome AS Atendente,
                i.Nome AS Inspetoria,
                atd.Data_Entrada,
                atd.Data_Inicial,
                atd.Data_Atendimento, 
                f.Nota
            FROM Atendimento atd
            INNER JOIN Profissional p ON atd.Profissional_Id = p.Id
            INNER JOIN Tipo t ON p.Tipo_Id = t.Id
            INNER JOIN Inspetoria i ON atd.Inspetoria_Id = i.Id
            LEFT JOIN Atendente at ON atd.Atendente_Id = at.Id
            LEFT JOIN Feedback f ON f.Atendimento_Id = atd.Id;";

                var queryResult = await connection.QueryAsync<AtendimentoDto>(sql);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento encontrado!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimentos listados com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }

        public async Task<ResponseModel<List<AtendimentoDto>>> BuscarAtendimentosPendentes()
        {
            var response = new ResponseModel<List<AtendimentoDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
              SELECT 
                atd.Id, 
                atd.Motivo, 
                CAST(atd.Protocolo AS VARCHAR) AS Protocolo,
                CAST(atd.Protocolo AS VARCHAR) AS Protocolo,
                p.Num_Cadastro_Nacional AS Profissional, 
                t.Nome AS Tipo, 
                at.Nome AS Atendente,
                i.Nome AS Inspetoria, 
                atd.Data_Entrada,
                atd.Data_Inicial, 
                atd.Data_Atendimento, 
                f.Nota
            FROM Atendimento atd
            INNER JOIN Profissional p ON atd.Profissional_Id = p.Id
            INNER JOIN Tipo t ON p.Tipo_Id = t.Id
            INNER JOIN Inspetoria i ON atd.Inspetoria_Id = i.Id
            LEFT JOIN Atendente at ON atd.Atendente_Id = at.Id
            LEFT JOIN Feedback f ON f.Atendimento_Id = atd.Id
            WHERE atd.Atendente_Id IS NULL;";

                var queryResult = await connection.QueryAsync<AtendimentoDto>(sql);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento pendente encontrado!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimentos pendentes listados com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }


        public async Task<ResponseModel<List<AtendimentoDto>>> BuscarAtendimentosPendentesPorInspetoria(int inspetoriaId)
        {
            var response = new ResponseModel<List<AtendimentoDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                            SELECT 
                                atd.Id, 
                                atd.Preferencial,
                                atd.Motivo, 
                                CAST(atd.Protocolo AS VARCHAR) AS Protocolo,
                                p.Num_Cadastro_Nacional AS Profissional, 
                                t.Nome AS Tipo, 
                                at.Nome AS Atendente,
                                i.Nome AS Inspetoria, 
                                atd.Data_Entrada, 
                                atd.Data_Inicial, 
                                atd.Data_Atendimento, 
                                f.Nota
                            FROM Atendimento atd
                            INNER JOIN Profissional p ON atd.Profissional_Id = p.Id
                            INNER JOIN Tipo t ON p.Tipo_Id = t.Id
                            INNER JOIN Inspetoria i ON atd.Inspetoria_Id = i.Id
                            LEFT JOIN Atendente at ON atd.Atendente_Id = at.Id
                            LEFT JOIN Feedback f ON f.Atendimento_Id = atd.Id
                            WHERE atd.Atendente_Id IS NULL
              AND atd.Inspetoria_Id = @InspetoriaId;";

                var queryResult = await connection.QueryAsync<AtendimentoDto>(sql, new { InspetoriaId = inspetoriaId });

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento pendente encontrado para essa Inspetoria!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimentos pendentes listados com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }


        public async Task<ResponseModel<List<AtendimentoDto>>> BuscarAtendimentosFeedbacks()
        {
            var response = new ResponseModel<List<AtendimentoDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                SELECT 
                    atd.Id, 
                    atd.Preferencial,
                    atd.Motivo, 
                    CAST(atd.Protocolo AS VARCHAR) AS Protocolo,
                    p.Num_Cadastro_Nacional AS Profissional, 
                    t.Nome AS Tipo, 
                    at.Nome AS Atendente,
                    i.Nome AS Inspetoria, 
                    atd.Data_Atendimento, 
                    f.Nota
                FROM Atendimento atd
                INNER JOIN Profissional p ON atd.Profissional_Id = p.Id
                INNER JOIN Tipo t ON p.Tipo_Id = t.Id
                INNER JOIN Inspetoria i ON atd.Inspetoria_Id = i.Id
                INNER JOIN Atendente at ON atd.Atendente_Id = at.Id
                INNER JOIN Feedback f ON f.Atendimento_Id = atd.Id;";

                var queryResult = await connection.QueryAsync<AtendimentoDto>(sql);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento com feedback encontrado!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimentos com feedback listados com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }

        // Buscar atendimento por Id com feedback
        public async Task<ResponseModel<AtendimentoDto>> BuscarAtendimentoPorId(int id)
        {
            var response = new ResponseModel<AtendimentoDto>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                    SELECT 
                        atd.Id, 
                        atd.Preferencial,
                        atd.Motivo, 
                        atd.Protocolo, 
                        p.Num_Cadastro_Nacional AS Profissional, 
                        t.Nome AS Tipo, 
                        at.Nome AS Atendente,
                        i.Nome AS Inspetoria, 
                        atd.Data_Atendimento, 
                        f.Nota
                    FROM Atendimento atd
                    LEFT JOIN Feedback f ON f.Atendimento_Id = atd.Id
                    INNER JOIN Profissional p ON atd.Profissional_Id = p.Id
                    INNER JOIN Tipo t ON p.Tipo_Id = t.Id
                    INNER JOIN Inspetoria i ON atd.Inspetoria_Id = i.Id
                    INNER JOIN Atendente at ON atd.Atendente_Id = at.Id
                    WHERE atd.Id = @Id";

                var result = await connection.QueryFirstOrDefaultAsync<AtendimentoDto>(sql, new { Id = id });

                if (result == null)
                {
                    response.Status = false;
                    response.Mensagem = "Atendimento não encontrado!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimento encontrado!";
                response.Dados = result;
            }

            return response;
        }

        // Criar novo atendimento
        public async Task<ResponseModel<List<Atendimento>>> CadastrarNovoAtendimento(Atendimento atendimento)
        {
            var response = new ResponseModel<List<Atendimento>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                    INSERT INTO Atendimento (Preferencial, Motivo, Protocolo, Data_Entrada, Data_Atendimento, Data_Inicial, Inspetoria_Id, Atendente_Id, Profissional_Id)
                    VALUES (@Preferencial, @Motivo, @Protocolo, @Data_Entrada, @Data_Atendimento, @Data_Inicial, @Inspetoria_Id, @Atendente_Id, @Profissional_Id);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var novoId = await connection.ExecuteScalarAsync<int>(sql, atendimento);

                if (novoId == 0)
                {
                    response.Status = false;
                    response.Mensagem = "Erro ao criar o atendimento!";
                    return response;
                }

                var novoAtendimento = await connection.QueryAsync<Atendimento>(
                    "SELECT * FROM Atendimento WHERE Id = @Id", new { Id = novoId });

                response.Dados = novoAtendimento.ToList();
                response.Status = true;
                response.Mensagem = "Atendimento criado com sucesso!";
            }

            return response;
        }

        // Editar atendimento
        public async Task<ResponseModel<List<Atendimento>>> EditarAtendimento(Atendimento atendimento)
        {
            var response = new ResponseModel<List<Atendimento>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                    UPDATE Atendimento 
                    SET Motivo = @Motivo, 
                        Protocolo = @Protocolo, 
                        Data_Entrada = @Data_Entrada,
                        Data_Atendimento = @Data_Atendimento,
                        Data_Inicial = @Data_Inicial,
                        Inspetoria_Id = @Inspetoria_Id, 
                        Atendente_Id = @Atendente_Id, 
                        Profissional_Id = @Profissional_Id
                    WHERE Id = @Id";

                var linhasAfetadas = await connection.ExecuteAsync(sql, atendimento);

                if (linhasAfetadas == 0)
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum registro foi atualizado!";
                    return response;
                }

                var atualizado = await connection.QueryAsync<Atendimento>(
                    "SELECT * FROM Atendimento WHERE Id = @Id", new { atendimento.Id });

                response.Status = true;
                response.Mensagem = "Atendimento atualizado com sucesso!";
                response.Dados = atualizado.ToList();
            }

            return response;
        }

        // Atualizar atendimento parcialmente (PATCH)
        public async Task<ResponseModel<List<Atendimento>>> AtualizarAtendimento(int id, Dictionary<string, object> campos)
        {
            var response = new ResponseModel<List<Atendimento>>();

            if (campos == null || campos.Count == 0)
            {
                response.Status = false;
                response.Mensagem = "Nenhum campo enviado para atualizar!";
                return response;
            }

            // 🔄 Converte JsonElement para tipos nativos (string, int, decimal, bool, null)
            var camposConvertidos = campos.ToDictionary(
                kvp => kvp.Key,
                kvp =>
                {
                    if (kvp.Value is JsonElement jsonElement)
                    {
                        return jsonElement.ValueKind switch
                        {
                            JsonValueKind.String => (object)jsonElement.GetString(),
                            JsonValueKind.Number => jsonElement.TryGetInt32(out int i) ? (object)i :
                                                      jsonElement.TryGetDecimal(out decimal d) ? (object)d : jsonElement.GetDouble(),
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
                var sql = $"UPDATE Atendimento SET {sets} WHERE Id = @Id";

                camposConvertidos.Add("Id", id);

                var linhasAfetadas = await connection.ExecuteAsync(sql, camposConvertidos);

                if (linhasAfetadas == 0)
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum registro foi atualizado!";
                    return response;
                }

                var atualizado = await connection.QueryAsync<Atendimento>(
                    "SELECT * FROM Atendimento WHERE Id = @Id", new { Id = id });

                response.Status = true;
                response.Mensagem = "Atendimento atualizado com sucesso!";
                response.Dados = atualizado.ToList();
            }

            return response;
        }

        // Remover atendimento
        public async Task<ResponseModel<Atendimento>> RemoverAtendimento(int id)
        {
            var response = new ResponseModel<Atendimento>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var existente = await connection.QueryFirstOrDefaultAsync<Atendimento>(
                    "SELECT * FROM Atendimento WHERE Id = @Id", new { Id = id });

                if (existente == null)
                {
                    response.Status = false;
                    response.Mensagem = "Atendimento não encontrado!";
                    return response;
                }

                var deletado = await connection.ExecuteAsync(
                    "DELETE FROM Atendimento WHERE Id = @Id", new { Id = id });

                if (deletado == 0)
                {
                    response.Status = false;
                    response.Mensagem = "Erro ao apagar o atendimento!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimento removido com sucesso!";
                response.Dados = existente;
            }

            return response;
        }
    }
}
