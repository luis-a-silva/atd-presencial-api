using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces

{
    public interface IAtendenteInterface
    {
        //Metodos.
        //tipo retorno, nome do metodo, parametros 

        Task<ResponseModel<List<Atendente>>> BuscarAtendentes();
        Task<ResponseModel<Atendente>> BuscarAtendentePorId(int id);
        Task<ResponseModel<List<Atendente>>> CriarNovoAtendente(Atendente atendente);
        Task<ResponseModel<List<Atendente>>> EditarAtendente(Atendente atendente);
        Task<ResponseModel<List<Atendente>>> AtualizarAtendente(int id, Dictionary<string, object> campos); // PATCH
        Task<ResponseModel<Atendente>> RemoverAtendente(int id);
    }
}
