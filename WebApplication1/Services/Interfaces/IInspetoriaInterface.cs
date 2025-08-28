using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IInspetoriaInterface
    {
        Task<ResponseModel<List<Inspetoria>>> BuscarInspetoria();
        Task<ResponseModel<Inspetoria>> BuscarInspetoriaPorId(int id);
        Task<ResponseModel<List<Inspetoria>>> CadastrarNovaInspetoria(Inspetoria inspetoria);
        Task<ResponseModel<List<Inspetoria>>> EditarInspetorias(Inspetoria inspetoria);
        Task<ResponseModel<Inspetoria>> RemoverInspetoria(int id);
    }
}
