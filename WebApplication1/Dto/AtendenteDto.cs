namespace WebApplication1.Models
{
    public class AtendenteDto
    {
        public int Id {  get; set; }
        
        public string Nome { get; set; }

        public string Email { get; set; }

        public int Inspetoria_Id { get; set; }

        public string Inspetoria { get; set; }

        public string Role { get; set; } = "user";
    }
}
