namespace WebApplication1.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int Nota { get; set; }
        public DateTime Data_Feedback { get; set; }
        public string ?Canal {  get; set; }
        public int Atendimento_Id { get; set; }

    }
}
