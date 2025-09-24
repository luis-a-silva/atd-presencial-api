using WebApplication1.Dto;
using WebApplication1.Models; // onde está o ResponseModel

namespace WebApplication1.Services.Interfaces
{

        public interface IDashboardInterface
        {
            Task<ResponseModel<TotalAtendimentosDto>> TotalAsync(int? inspetoriaId = null);
            Task<ResponseModel<List<AtendimentosPorInspetoriaDto>>> PorInspetoriaAsync(int? inspetoriaId = null);
            Task<ResponseModel<List<AtendimentosPorMotivoDto>>> PorMotivoAsync(int? inspetoriaId = null);
            Task<ResponseModel<List<AtendimentosPorTipoDto>>> PorTipoAsync(int? inspetoriaId = null);
            Task<ResponseModel<List<SerieMensalDto>>> SerieMensalAsync(int anoInicio, int anoFim, int? inspetoriaId = null);
        }

    
}