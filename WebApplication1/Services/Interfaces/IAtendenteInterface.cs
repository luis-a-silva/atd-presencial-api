using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces

{
    public interface IAtendenteInterface
    {
        //Metodos.
        //tipo retorno, nome do metodo, parametros 

        Task<ResponseModel<List<AtendenteDto>>> BuscarAtendentes();
        Task<ResponseModel<AtendenteDto>> BuscarAtendentePorId(int id);
        Task<ResponseModel<List<AtendenteDto>>> CriarNovoAtendente(Atendente atendente);
        Task<ResponseModel<List<Atendente>>> EditarAtendente(Atendente atendente);
        Task<ResponseModel<List<Atendente>>> AtualizarAtendente(int id, Dictionary<string, object> campos); // PATCH
        Task<ResponseModel<Atendente>> RemoverAtendente(int id);
    }
}
