using WebApplication1.Dto;
using WebApplication1.Models; // onde está o ResponseModel

namespace WebApplication1.Services.Interfaces
{
    public interface IDashboardInterface
    {
        Task<ResponseModel<TotalAtendimentosDto>> TotalAsync();
        Task<ResponseModel<List<AtendimentosPorInspetoriaDto>>> PorInspetoriaAsync();
        Task<ResponseModel<List<AtendimentosPorMotivoDto>>> PorMotivoAsync();
        Task<ResponseModel<List<AtendimentosPorTipoDto>>> PorTipoAsync();
        Task<ResponseModel<List<SerieMensalDto>>> SerieMensalAsync(int anoInicio, int anoFim);
    }
}
