namespace WebApplication1.Models
{
    public class Atendimento
    {
        public int Id { get; set; }
        public string Motivo { get; set; }
        public long Protocolo { get; set; }
        public DateTime Data_Atendimento { get; set; }
        public int Inspetoria_Id { get; set; }
        public int? Atendente_Id { get; set; }
        public int Profissional_Id { get; set; }
    }
}
