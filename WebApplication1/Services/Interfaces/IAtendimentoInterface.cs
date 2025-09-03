using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IAtendimentoInterface
    {
        Task<ResponseModel<AtendimentoDto>> BuscarAtendimentoPorId(int id);
        Task<ResponseModel<List<Atendimento>>> CadastrarNovoAtendimento(Atendimento atendimento);
        Task<ResponseModel<List<Atendimento>>> EditarAtendimento(Atendimento atendimento);
        Task<ResponseModel<List<Atendimento>>> AtualizarAtendimento(int id, Dictionary<string, object> campos); // PATCH
        Task<ResponseModel<Atendimento>> RemoverAtendimento(int id);
        Task<ResponseModel<List<AtendimentoDto>>> BuscarTodosAtendimentos();
        Task<ResponseModel<List<AtendimentoDto>>> BuscarAtendimentosPendentes();
        Task<ResponseModel<List<AtendimentoDto>>> BuscarAtendimentosFeedbacks();
        Task<ResponseModel<List<AtendimentoDto>>> BuscarAtendimentosPendentesPorInspetoria(int atendenteId);
    }
}
