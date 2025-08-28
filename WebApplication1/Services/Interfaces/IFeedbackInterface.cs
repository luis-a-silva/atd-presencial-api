using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IFeedbackInterface

    {
        Task<ResponseModel<List<Feedback>>> ReceberFeedback(Feedback feedback);
    }
}
