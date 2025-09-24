using System;
using System.Text.Json.Serialization;

namespace WebApplication1.Dto
{
    public sealed class AtendimentoCaptchaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("motivo")]
        public string Motivo { get; set; }

        [JsonPropertyName("preferencial")]
        public bool Preferencial { get; set; }

        [JsonPropertyName("protocolo")]
        public string Protocolo { get; set; }

        [JsonPropertyName("data_entrada")]
        public DateTime Data_Entrada{ get; set; }

        [JsonPropertyName("data_inicial")]
        public DateTime? Data_Inicial{ get; set; }

        [JsonPropertyName("data_atendimento")]
        public DateTime? Data_Atendimento { get; set; }

        [JsonPropertyName("inspetoria_id")]
        public int Inspetoria_Id { get; set; }

        [JsonPropertyName("atendente_id")]
        public int? Atendente_Id { get; set; }

        [JsonPropertyName("profissional_id")]
        public int Profissional_Id { get; set; }

        // 🔹 Novo campo para reCAPTCHA
        [JsonPropertyName("recaptchaToken")]
        public string RecaptchaToken { get; set; }
    }
}
