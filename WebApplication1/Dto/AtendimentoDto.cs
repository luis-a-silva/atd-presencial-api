using System.Text.Json.Serialization;

namespace WebApplication1.Dto
{
    public sealed class AtendimentoDto
    {
    [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("motivo")]
        public string Motivo { get; set; }

        [JsonPropertyName("protocolo")]
        public string Protocolo { get; set; }

        [JsonPropertyName("profissional")]
        public string Profissional { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("atendente")]
        public string Atendente { get; set; }

        [JsonPropertyName("inspetoria")]
        public string Inspetoria { get; set; }

        [JsonPropertyName("data_entrada")]
        public DateTime Data_Entrada { get; set; }

        [JsonPropertyName("data_inicial")]
        public DateTime? Data_Inicial { get; set; }

        [JsonPropertyName("data_atendimento")]
        public DateTime? Data_Atendimento { get; set; }

        [JsonPropertyName("nota")]
        public int? Nota { get; set; }
    }

}

