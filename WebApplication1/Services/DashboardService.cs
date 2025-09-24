using Microsoft.Data.SqlClient;
using Dapper;
using WebApplication1.Dto;
using WebApplication1.Services.Interfaces;
using WebApplication1.Models; // onde está o ResponseModel

namespace WebApplication1.Services
{
    public class DashboardService : IDashboardInterface
    {
        private readonly IConfiguration _configuration;

        public DashboardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel<TotalAtendimentosDto>> TotalAsync(int? inspetoriaId = null)
        {
            var response = new ResponseModel<TotalAtendimentosDto>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                            SELECT COUNT(1) AS Total_Atendimentos
                            FROM Atendimento a
                            /**where**/;";

                var builder = new SqlBuilder();
                var template = builder.AddTemplate(sql);

                if (inspetoriaId.HasValue)
                    builder.Where("a.Inspetoria_Id = @inspetoriaId", new { inspetoriaId });

                var queryResult = await connection.QuerySingleOrDefaultAsync<TotalAtendimentosDto>(template.RawSql, template.Parameters);

                if (queryResult == null)
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum dado encontrado!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Total de atendimentos listado com sucesso!";
                response.Dados = queryResult;
            }

            return response;
        }

        // 2. Atendimentos por Inspetoria
        public async Task<ResponseModel<List<AtendimentosPorInspetoriaDto>>> PorInspetoriaAsync(int? inspetoriaId = null)
        {
            var response = new ResponseModel<List<AtendimentosPorInspetoriaDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                            SELECT i.Nome AS Inspetoria_Nome, COUNT(a.Id) AS Total_Atendimentos
                            FROM Atendimento a
                            JOIN Inspetoria i ON i.Id = a.Inspetoria_Id
                            /**where**/
                            GROUP BY i.Nome
                            ORDER BY i.Nome;";

                var builder = new SqlBuilder();
                var template = builder.AddTemplate(sql);

                if (inspetoriaId.HasValue)
                    builder.Where("a.Inspetoria_Id = @inspetoriaId", new { inspetoriaId });

                var queryResult = await connection.QueryAsync<AtendimentosPorInspetoriaDto>(template.RawSql, template.Parameters);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento encontrado por Inspetoria!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimentos por Inspetoria listados com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }

        // 3. Atendimentos por Motivo
        public async Task<ResponseModel<List<AtendimentosPorMotivoDto>>> PorMotivoAsync(int? inspetoriaId = null)
        {
            var response = new ResponseModel<List<AtendimentosPorMotivoDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                        SELECT a.Motivo, COUNT(*) AS Total_Atendimentos
                        FROM Atendimento a
                        /**where**/
                        GROUP BY a.Motivo;";

                var builder = new SqlBuilder();
                var template = builder.AddTemplate(sql);

                if (inspetoriaId.HasValue)
                    builder.Where("a.Inspetoria_Id = @inspetoriaId", new { inspetoriaId });

                var queryResult = await connection.QueryAsync<AtendimentosPorMotivoDto>(template.RawSql, template.Parameters);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento encontrado por Motivo!";
                    return response;
                }

                // Processamento dos motivos separados
                var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in queryResult)
                {
                    if (string.IsNullOrWhiteSpace(row.Motivo)) continue;

                    var motivos = row.Motivo.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var m in motivos.Select(m => m.Trim()))
                    {
                        if (!dict.ContainsKey(m))
                            dict[m] = 0;

                        dict[m] += row.Total_Atendimentos;
                    }
                }

                response.Status = true;
                response.Mensagem = "Atendimentos por Motivo listados com sucesso!";
                response.Dados = dict.Select(x => new AtendimentosPorMotivoDto
                {
                    Motivo = x.Key,
                    Total_Atendimentos = x.Value
                }).OrderByDescending(x => x.Total_Atendimentos).ToList();
            }

            return response;
        }

        // 4. Atendimentos por Tipo (CPF/CNPJ)
        public async Task<ResponseModel<List<AtendimentosPorTipoDto>>> PorTipoAsync(int? inspetoriaId = null)
        {
            var response = new ResponseModel<List<AtendimentosPorTipoDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                            WITH tipos AS (SELECT 1 AS tipo_id UNION ALL SELECT 2),
                            contagem AS (
                                SELECT p.tipo_id, COUNT(a.id) AS total
                                FROM Atendimento a
                                JOIN Profissional p ON p.id = a.profissional_id
                                /**where**/
                                GROUP BY p.tipo_id
                            )
                            SELECT t.tipo_id AS Tipo_Profissional,
                                   COALESCE(c.total, 0) AS Total_Atendimentos
                            FROM tipos t
                            LEFT JOIN contagem c ON c.tipo_id = t.tipo_id
                            ORDER BY t.tipo_id;";

                var builder = new SqlBuilder();
                var template = builder.AddTemplate(sql);

                if (inspetoriaId.HasValue)
                    builder.Where("a.Inspetoria_Id = @inspetoriaId", new { inspetoriaId });

                var queryResult = await connection.QueryAsync<AtendimentosPorTipoDto>(template.RawSql, template.Parameters);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento encontrado por Tipo!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Atendimentos por Tipo listados com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }

        // 5. Série Mensal
        public async Task<ResponseModel<List<SerieMensalDto>>> SerieMensalAsync(int anoInicio, int anoFim, int? inspetoriaId = null)
        {
            var response = new ResponseModel<List<SerieMensalDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                            SELECT YEAR(a.Data_Atendimento) AS Ano,
                                   MONTH(a.Data_Atendimento) AS Mes,
                                   COUNT(*) AS Total_Atendimentos
                            FROM Atendimento a
                            /**where**/
                            AND YEAR(a.Data_Atendimento) BETWEEN @anoInicio AND @anoFim
                            GROUP BY YEAR(a.Data_Atendimento), MONTH(a.Data_Atendimento)
                            ORDER BY Ano, Mes;";

                var builder = new SqlBuilder();
                builder.Where("1=1"); // base
                if (inspetoriaId.HasValue)
                    builder.Where("a.Inspetoria_Id = @inspetoriaId", new { inspetoriaId });

                var template = builder.AddTemplate(sql, new { anoInicio, anoFim });

                var queryResult = await connection.QueryAsync<SerieMensalDto>(template.RawSql, template.Parameters);

                if (!queryResult.Any())
                {
                    response.Status = false;
                    response.Mensagem = "Nenhum atendimento encontrado para a série mensal!";
                    return response;
                }

                response.Status = true;
                response.Mensagem = "Série mensal listada com sucesso!";
                response.Dados = queryResult.ToList();
            }

            return response;
        }

    }
}

