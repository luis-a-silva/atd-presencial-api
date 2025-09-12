namespace WebApplication1.Dto
{
    public class DashboardDto
    {
    }
    public sealed class TotalAtendimentosDto
    {
        public int Total_Atendimentos { get; set; }
    }

    public sealed class AtendimentosPorInspetoriaDto
    {
        public string Inspetoria_Nome { get; set; }
        public int Total_Atendimentos { get; set; }
    }

    public sealed class AtendimentosPorMotivoDto
    {
        public string Motivo { get; set; }
        public int Total_Atendimentos { get; set; }
    }

    public sealed class SerieMensalDto
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int Total_Atendimentos { get; set; }
    }

    public sealed class AtendimentosPorTipoDto
    {
        public string Tipo_Profissional { get; set; }
        public int Total_Atendimentos { get; set; }
    }


}
