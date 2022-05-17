namespace AgendaMedica.Models
{
    public class Doutor
    {
        public Doutor(){
            diario = new List<Agenda>();
        }
        public Doutor(string Nome, int CRM)
        {
            this.crm = CRM; 
            this.nome = Nome;               
            diario = new List<Agenda>();        
        }

        public string? nome { get; set; }

        public int crm { get; set;}

        public List<Agenda> diario { get; set; }
    }
}