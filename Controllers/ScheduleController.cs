using Microsoft.AspNetCore.Mvc;
using AgendaMedica.Models;
using AgendaMedica.Service;

namespace AgendaMedica.Controllers;

[ApiController]
[Route("api/agendas")]
public class ScheduleController : ControllerBase
    {
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService){
        _scheduleService = scheduleService;
    }  
    
    [HttpGet("listarDoutores")]
    public ActionResult<List<Doutor>> listaMedicos(){ 
        List<Doutor> lista = _scheduleService.listDocs(); 
        if(!lista.Any()){
            return NotFound("Nenhum médico encontrado.");
        }      
                  
        return Ok(lista);
    }

    // WIP

    [HttpPatch("{CRM}/otimizar")]
    public ActionResult<List<Agenda>[]> otimizarAgenda(int CRM){     
        Doutor oldDoc = _scheduleService.getDoc(CRM);
        try{
            _scheduleService.optimize(CRM);
        }
        catch(ArgumentOutOfRangeException)
        {
            return StatusCode(500, "Esse médico não pode receber mais cirurgias.");
        }
        catch(ArgumentNullException)
        {
            return StatusCode(404, "Não foi possível remover o agendamento informado.");
        }
        catch(ArgumentException)
        {
            return StatusCode(500, "Informe uma data válida.");
        } 
        catch(IndexOutOfRangeException)
        {
            return StatusCode(400, "Esta agenda médica está cheia.");
        }
        catch(InvalidDataException)
        {
            return StatusCode(500, "Campos obrigatórios vazios ou inválidos.");
        }
        catch(NullReferenceException)
        {
            return StatusCode(404, "Médico e/ou agenda médica não encontrado(s).");
        }
        
        return oldDoc.diario;
    }

    [HttpPost]
    public ActionResult<bool> adicionarMedico([FromBody]Doutor doc){   
        try
        {
            _scheduleService.newDoc(doc);
        }
        catch(ArgumentNullException){
            return StatusCode(500, "Informe um objeto.");
        }
        catch(InvalidDataException){
            return StatusCode(500, "Campos obrigatórios vazios ou inválidos.");
        }
        catch(InvalidOperationException){
            return StatusCode(406, "Já existe um doutor com esse CRM.");
        }
        return Ok(true);
    }
    
    [HttpPost("{CRM}/agendar")]
    public ActionResult<List<Agenda>[]> agendar(int CRM, [FromBody] Agenda agenda){                       
        Doutor oldDoc = _scheduleService.getDoc(CRM);
        try
        {
            _scheduleService.saveAttendance(CRM, agenda);               
        }      
        catch(ArgumentOutOfRangeException)
        {
            return StatusCode(500, "Esse médico não pode receber mais cirurgias.");
        }                   
        catch(ArgumentException)
        {
            return StatusCode(500, "Informe uma data válida.");
        } 
        catch(IndexOutOfRangeException)
        {
            return StatusCode(400, "Esta agenda médica está cheia.");
        }
        catch(InvalidDataException)
        {
            return StatusCode(500, "Campos obrigatórios vazios ou inválidos.");
        }
        catch(NullReferenceException)
        {
            return StatusCode(404, "Médico e/ou agenda médica não encontrado(s).");
        }
                                 
        return Ok(oldDoc.diario);
    }

    [HttpPatch("{CRM}/{Id}")]
    public ActionResult<bool> alterarAgendamento(int CRM, int Id, [FromBody] Agenda agenda){  
        if(agenda == null || agenda.dia == null){
            return StatusCode(400, "Campos obrigatórios vazios ou inválidos.");
        }   
        try
        {        
            _scheduleService.changeAppointment(CRM, Id, agenda);            
        }
        catch(ArgumentNullException)
        {
            return StatusCode(404, "Não foi possível remover o agendamento informado.");
        }
        catch(ArgumentOutOfRangeException)
        {
            return StatusCode(500, "Esse médico não pode receber mais cirurgias nesse dia.");
        }           
        catch(IndexOutOfRangeException)
        {
            return StatusCode(400, "A agenda médica desse dia está cheia.");
        }
        catch(InvalidDataException)
        {
            return StatusCode(500, "Informe uma data válida.");
        }
        catch(NullReferenceException)
        {
            return StatusCode(404, "Médico e/ou agenda médica não encontrado(s).");
        }
                      
        return Ok(true);
    }        

    [HttpDelete("{CRM}")]
    public ActionResult<bool> removerMedico(int CRM){      
        try{
            _scheduleService.deleteDoc(CRM);
        }
        catch(NullReferenceException)
        {
            return StatusCode(404, "Médico não encontrado.");
        }
        catch(InvalidOperationException)
        {
            return StatusCode(400, "A agenda desse médico não está vazia.");
        }
        return Ok(true);
    }

    [HttpDelete("{CRM}/{Id}")]
    public ActionResult<bool> cancelarAgendamento(int CRM, int Id){
        try
        {
            _scheduleService.cancelAppointment(CRM, Id);
        }              
        catch(NullReferenceException)
        {
            return StatusCode(404, "Médico e/ou agenda médica não encontrado(s).");
        }
        catch(ArgumentNullException)
        {
            return StatusCode(400, "Não foi possível remover o agendamento.");
        }
        catch(SystemException)
        {
            return StatusCode(500, "Só podem ser canceladas consultas com antecedência de 3 dias.");
        }
                   
        return Ok(true);
        
    } 
    
}
