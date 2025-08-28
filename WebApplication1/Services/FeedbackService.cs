using Dapper;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

public class FeedbackService : IFeedbackInterface
{
    private readonly IConfiguration _configuration;

    public FeedbackService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ResponseModel<List<Feedback>>> ReceberFeedback(Feedback feedback)
    {
        var response = new ResponseModel<List<Feedback>>();

        if (feedback == null)
        {
            response.Status = false;
            response.Mensagem = "Feedback inválido!";
            return response;
        }

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            var sql = @"
                INSERT INTO Feedback (Nota, Data_Feedback, Canal, Atendimento_Id)
                VALUES (@Nota, @Data_Feedback, @Canal, @Atendimento_Id);

                SELECT * FROM Feedback WHERE Id = SCOPE_IDENTITY();
            ";

            // Caso Data_Feedback não seja enviado, atribui DataTime.Now
            if (feedback.Data_Feedback == default)
                feedback.Data_Feedback = DateTime.Now;

            var result = await connection.QueryAsync<Feedback>(sql, feedback);

            response.Status = true;
            response.Mensagem = "Feedback recebido com sucesso!";
            response.Dados = result.ToList();
        }

        return response;
    }
}
