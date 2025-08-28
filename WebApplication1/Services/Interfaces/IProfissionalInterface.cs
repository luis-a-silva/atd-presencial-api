using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IProfissionalInterface
    {
        Task<ResponseModel<List<Profissional>>> CadastrarProfissional(Profissional profissional);

    }
}
