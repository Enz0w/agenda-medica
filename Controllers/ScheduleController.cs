using Microsoft.AspNetCore.Mvc;
using AgendaMedica.Models;

namespace AgendaMedica.Controllers;

[ApiController]
[Route("api/agenda")]
public class ScheduleController : ControllerBase
    {
        private static int countId = 1;
    
    private static List<Doutor> listaDoutores = new List<Doutor>(); 

    private static List<Agenda> listaAgendamento = new List<Agenda>();

    private static DateTime hoje = DateTime.Now;     

    [HttpGet]
    public string Get(){
        return "Agendamento médico";
    }

    [HttpGet("listarDoutores")]
    public ActionResult<List<Doutor>> listaMedicos(){  
        if(!listaDoutores.Any() || listaDoutores == null){
            return StatusCode(200, "A lista de médicos está vazia.");
        }      
                  
        return Ok(listaDoutores);
    }

    [HttpPost]
    public ActionResult<bool> adicionarMedico([FromBody]Doutor doc){   
        if(doc == null){
            return StatusCode(500, "Informe um objeto.");
        }   
        if(doc.nome == "" || doc.nome == null || doc.crm < 1000 || doc.crm > 9999){
            return StatusCode(500, "Campos obrigatórios vazios ou inválidos.");
        }        
        listaDoutores.Add(doc);
        return Ok(true);
    }
    
    [HttpPost("agendar/{CRM}")]
    public ActionResult<List<Agenda>> agendar(int CRM, [FromBody] Agenda agenda){
        Doutor oldDoc = getDocByCRM(CRM);
        
        if(oldDoc == null){
            return StatusCode(404, "Médico não encontrado.");
        }
        if(agenda == null || agenda.atendimento < 1 || agenda.atendimento > 2){
            return StatusCode(204, "Campos obrigatórios vazios ou inválidos.");
        }
        if(oldDoc.diario.Count >= 5){
            return StatusCode(500, "A agenda desse médico está cheia.");
        }
        if(agenda.atendimento == 2 && oldDoc.diario.Any(agenda => agenda.atendimento == 2)){
            return StatusCode(500, "Esse médico não pode receber mais cirurgias.");
        }
        if(agenda.dia < hoje || agenda.dia.DayOfWeek == DayOfWeek.Saturday || agenda.dia.DayOfWeek == DayOfWeek.Sunday){
            return StatusCode(500, "Informe uma data válida.");
        }
        else{
            agenda.doutor = oldDoc.nome;
            agenda.id = countId++;
            oldDoc.diario.Add(agenda);
            agenda.data = agenda.dia.ToString("dd-MM-yyyy");
        }                          
                                 
        return Ok(oldDoc.diario);
    }

    [HttpPatch("{CRM}/{Id}")]
    public ActionResult<bool> alterarAgendamento(int CRM, int Id, Agenda agenda){
        Doutor oldDoc = getDocByCRM(CRM);        
        Agenda agendaDoc = getDayById(CRM, Id);
        if(oldDoc == null){
            return StatusCode(404, "Médico não encontrado.");
        }
        if(agendaDoc == null){
            return StatusCode(404, "Agenda médica não encontrada.");
        }
        if(agenda == null || agenda.dia == null){
            return StatusCode(204, "Campos obrigatórios vazios ou inválidos");
        }
        if(agenda.dia < hoje || agenda.dia.DayOfWeek == DayOfWeek.Saturday || agenda.dia.DayOfWeek == DayOfWeek.Sunday){
            return StatusCode(500, "Informe uma data válida");
        }
        else{
            agendaDoc.dia = agenda.dia;
            agendaDoc.data = agenda.dia.ToString("dd-MM-yyyy");
        }
        return Ok(true);
    }        

    [HttpDelete("{CRM}")]
    public ActionResult<bool> removerMedico(int CRM){
        Doutor aux = null;
        Doutor oldDoc = getDocByCRM(CRM);

        if(oldDoc == null){
            return StatusCode(404, "Médico não encontrado.");
        }
        else{
            aux = oldDoc;
        }
        if(aux != null){
            listaDoutores.Remove(aux);
            return Ok(true);
        }
        return NotFound(false);
    }

    [HttpDelete("{CRM}/{Id}")]
    public ActionResult<bool> cancelarAgendamento(int CRM, int Id){
        Doutor oldDoc = getDocByCRM(CRM);
        Agenda agendaDoc = getDayById(CRM, Id);
        TimeSpan diferençaDias = agendaDoc.dia - hoje;
        int totalDiff = diferençaDias.Days;      
        if(oldDoc == null){
            return StatusCode(404, "Médico não encontrado.");
        }
        if(agendaDoc == null){
            return StatusCode(404, "Agenda não encontrada.");
        }
        if(agendaDoc.atendimento == 1 && (totalDiff < 3)){
            return StatusCode(500, "Só podem ser canceladas consultas com antecedência de 3 dias.");
        }
        else{
            oldDoc.diario.Remove(agendaDoc);
            return Ok(true);
        }
    }

    private Doutor getDocByCRM(int CRM){
        foreach(Doutor doutor in listaDoutores){
            if(doutor.crm == CRM){
                return doutor;
            }
        }
        return null;
    }
    private Agenda getDayById(int CRM, int Id){
        Doutor oldDoc = getDocByCRM(CRM); 
        if(oldDoc == null){
            return null;
        }        
        List<Agenda> horarios = oldDoc.diario;    
        foreach(Agenda horario in horarios){
            if(horario.id == Id){
                return horario;
            }            
        }
        return null;
    }
}
