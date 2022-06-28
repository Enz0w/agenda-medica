using System.Globalization;
namespace AgendaMedica.Models
{
    public class Agenda
    {
        public Agenda(){
            dia = new DateTime();                  
        }        
        public Agenda(int Id, string nome_doutor, int tipoAtendimento) : this()
        {
            this.id = Id;
            this.doutor = nome_doutor;
            this.atendimento = tipoAtendimento;                                   
        }   

        public int id { get; set; }        

        public string? doutor { get; set; }

        public int atendimento { get; set; }    
        public string? diaDaSemana { get; set; }         
        public DateTime dia { get; set; } 
    }
}