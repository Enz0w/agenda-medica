using AgendaMedica.Models;
namespace AgendaMedica.Service;

public class ScheduleService : IScheduleService
{
    private static int countId = 1;
    
    private static List<Doutor> listaDoutores = new List<Doutor>(); 

    private static DateTime hoje = DateTime.Now; 

    // Otimização de uma agenda, a otimização acontece na tentativa de antecipar os compromissos
    // de um médico, conforme o processo acontece são verificadas as mesmas políticas de agendamento 
    // e cancelamento.
    // AVISO: NÃO ESTÁ FUNCIONANDO 100%
    
    public List<Agenda>[] optimize(int CRM){
        Doutor oldDoc = getDoc(CRM);
        Agenda aux;   
                 
        if(oldDoc.diario[0] == null && oldDoc.diario[1] != null || oldDoc.diario[0].Count < oldDoc.diario[1].Count && oldDoc.diario[1].First().atendimento != 2){
            aux = oldDoc.diario[1].First();
            aux.dia = aux.dia.AddDays(-1);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);           
        }
        else if(oldDoc.diario[0] == null && oldDoc.diario[2] != null || oldDoc.diario[0].Count < oldDoc.diario[2].Count && oldDoc.diario[2].First().atendimento != 2){
            aux = oldDoc.diario[2].First();
            aux.dia = aux.dia.AddDays(-2);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        }
        else if(oldDoc.diario[0] == null && oldDoc.diario[3] != null || oldDoc.diario[0].Count < oldDoc.diario[3].Count && oldDoc.diario[3].First().atendimento != 2){
            aux = oldDoc.diario[3].First();
            aux.dia = aux.dia.AddDays(-3);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        }
        else if(oldDoc.diario[0] == null && oldDoc.diario[4] != null || oldDoc.diario[0].Count < oldDoc.diario[4].Count && oldDoc.diario[4].First().atendimento != 2){
            aux = oldDoc.diario[4].First();
            aux.dia = aux.dia.AddDays(-4);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        }     
        else if(oldDoc.diario[1] == null && oldDoc.diario[2] != null || oldDoc.diario[1].Count < oldDoc.diario[2].Count && oldDoc.diario[4].First().atendimento != 2){
            aux = oldDoc.diario[2].First();
            aux.dia = aux.dia.AddDays(-1);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        }  
        else if(oldDoc.diario[1] == null && oldDoc.diario[3] != null || oldDoc.diario[1].Count < oldDoc.diario[3].Count && oldDoc.diario[3].First().atendimento != 2){
            aux = oldDoc.diario[3].First();
            aux.dia = aux.dia.AddDays(-2);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        } 
        else if(oldDoc.diario[1] == null && oldDoc.diario[4] != null || oldDoc.diario[1].Count < oldDoc.diario[4].Count && oldDoc.diario[4].First().atendimento != 2){
            aux = oldDoc.diario[4].First();
            aux.dia = aux.dia.AddDays(-3);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        } 
        else if(oldDoc.diario[2] == null && oldDoc.diario[3] != null || oldDoc.diario[2].Count < oldDoc.diario[3].Count && oldDoc.diario[3].First().atendimento != 2){
            aux = oldDoc.diario[3].First();
            aux.dia = aux.dia.AddDays(-1);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        } 
        else if(oldDoc.diario[2] == null && oldDoc.diario[4] != null || oldDoc.diario[2].Count < oldDoc.diario[4].Count && oldDoc.diario[4].First().atendimento != 2){
            aux = oldDoc.diario[4].First();
            aux.dia = aux.dia.AddDays(-2);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        } 
        else if(oldDoc.diario[3] == null && oldDoc.diario[4] != null || oldDoc.diario[3].Count < oldDoc.diario[4].Count && oldDoc.diario[4].First().atendimento != 2){
            aux = oldDoc.diario[4].First();
            aux.dia = aux.dia.AddDays(-1);
            cancel(CRM, aux.id);
            saveAttendance(CRM, aux);
        } 
        
        return oldDoc.diario;
    }      
    
    // Obtém a lista de doutores
    public List<Doutor> listDocs(){  
        if(listaDoutores == null){
            return new List<Doutor>();
        }      
               
        return listaDoutores;
    }
    
    // Adiciona um novo doutor à lista de doutores.
    public bool newDoc(Doutor doc){   
        if(doc == null){
            throw new ArgumentNullException();
        }   
        if(doc.nome == "" || doc.nome == null || doc.crm < 1000 || doc.crm > 9999){
            throw new InvalidDataException();
        }
        // Verifica se já não existe um doutor com esse CRM.
        if(listaDoutores.Find(aux => aux.crm == doc.crm) != null){
            throw new InvalidOperationException();
        }        
        listaDoutores.Add(doc);
        return true;
    }

    // Remove o doutor com CRM informado da lista de doutores.
    public bool deleteDoc(int CRM){
        Doutor oldDoc = getDoc(CRM);
        Doutor aux;
        if(oldDoc == null){
            throw new NullReferenceException();
        }
        else{
            aux = oldDoc;
        }              
        // Verifica se a agenda do doutor está vazia.
        if(oldDoc.diario != null){
            throw new InvalidOperationException();   
        }
        listaDoutores.Remove(aux);
        return true;      
    }

    // Altera a data de um compromisso utilizando dos dados informados.
    public bool changeAppointment(int CRM, int Id, Agenda agenda){
        Doutor oldDoc = getDoc(CRM);           
        Agenda agendaDoc = getAppointment(CRM, Id);
        if(oldDoc == null || agendaDoc == null){
            throw new NullReferenceException();
        }
        
        // Verifica se a data inserida é válida, se é maior que o dia de hoje ou se não é domingo ou sábado.
        if(agenda.dia < hoje && !agenda.dia.Equals(hoje) || agenda.dia.DayOfWeek == DayOfWeek.Saturday || agenda.dia.DayOfWeek == DayOfWeek.Sunday){
            throw new InvalidDataException();            
        }
        try
        {
            agenda.atendimento = agendaDoc.atendimento;
            saveAttendance(CRM, agenda);
            cancel(CRM, Id);
        }
        catch(ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException();
        }           
        catch(ArgumentNullException)
        {
            throw new ArgumentNullException();
        }
        catch(IndexOutOfRangeException)
        {
            throw new IndexOutOfRangeException();
        }
                      
        return true;
    }
    
    // Cancela um agendamento, atribuído ao doutor do CRM e Id passados como parâmetros,
    // novamente seguindo padrões de políticas do estabelecimento frequentado.
    public bool cancelAppointment(int CRM, int Id){
        Doutor oldDoc = getDoc(CRM);
        Agenda agendaDoc = getAppointment(CRM, Id);        
        
        if(oldDoc == null || agendaDoc == null){            
            throw new NullReferenceException();
        }        
        // Obtém a diferença de dias e atribui a uma variável int com o método Days.
        TimeSpan diferençaDias = agendaDoc.dia - hoje;              
        int totalDiff = diferençaDias.Days; 

        // Verifica se a consulta a ser cancelada tem total de dias menor que 3.
        if(agendaDoc.atendimento == 1 && (totalDiff < 3)){
            throw new SystemException();
        }     
        try
        {
            cancel(CRM, Id);
        }
        catch(ArgumentNullException){
            throw new ArgumentNullException();
        }                              
                   
        return true;
        
    }

    // Verifica se existe um doutor com tal CRM e o retorna caso encontre.
    public Doutor getDoc(int CRM){
        foreach(Doutor doutor in listaDoutores){
            if(doutor.crm == CRM){
                return doutor;
            }
        }
        return null;
    }

    // Obtém determinado agendamento com base no Id passado como parâmetro.
    public Agenda getAppointment(int CRM, int Id){
        Doutor oldDoc = getDoc(CRM); 
        List<Agenda>[] horarios = oldDoc.diario; 
        if(oldDoc == null){
            return null;
        }        
        // Percorre o diario do doutor informado, ao encontrar o agendamento retorna, caso não encontrado retorna nulo.  
        foreach(List<Agenda> horario in horarios){
            if(horario != null){
                foreach(Agenda agendamento in horario){
                    if(agendamento.id == Id){                    
                        return agendamento;
                    }           
                }
            }                                   
        }
        return null;
        
    }

    // Criar um novo atendimento, com base nas diretivas e políticas da clínica ou hospital
    // é feito um agendamento no diario do Doutor.
    public bool saveAttendance(int CRM, Agenda agenda){
        Doutor oldDoc = getDoc(CRM);
        agenda.doutor = oldDoc.nome;    
        if(oldDoc == null || agenda == null){
            throw new NullReferenceException();
        }
        if(agenda.atendimento < 1 || agenda.atendimento > 2){
            throw new InvalidDataException();
        } 
        if(agenda.dia < hoje && !agenda.dia.Equals(hoje) || agenda.dia.DayOfWeek == DayOfWeek.Saturday || agenda.dia.DayOfWeek == DayOfWeek.Sunday){
            throw new ArgumentException();
        }    
        switch(agenda.dia.DayOfWeek){
            case DayOfWeek.Monday:
                agenda.diaDaSemana = "Segunda-feira";                   
                    
                if(oldDoc.diario[0] == null){
                    agenda.id = countId++;
                    oldDoc.diario[0] = new List<Agenda> {agenda};      
                                         
                }
                else if(oldDoc.diario[0] != null){
                    if(oldDoc.diario[0].Count() == 5){
                        throw new IndexOutOfRangeException();                            
                    }
                    if(agenda.atendimento == 2 && oldDoc.diario[0].Any(agenda => agenda.atendimento == 2)){
                        throw new ArgumentOutOfRangeException();
                    }
                    else{
                        agenda.id = countId++;
                        oldDoc.diario[0].Add(agenda);
                    }                                      
                }                                  
                break;
            case DayOfWeek.Tuesday:
                agenda.diaDaSemana = "Terça-feira";

                if(oldDoc.diario[1] == null){
                    agenda.id = countId++;
                    oldDoc.diario[1] = new List<Agenda> {agenda};      
                }
                else if(oldDoc.diario[1] != null){
                    if(oldDoc.diario[1].Count() == 5){
                        throw new IndexOutOfRangeException();
                    }
                    if(agenda.atendimento == 2 && oldDoc.diario[1].Any(agenda => agenda.atendimento == 2)){
                        throw new ArgumentOutOfRangeException();
                    }
                    else{
                        agenda.id = countId++;
                        oldDoc.diario[1].Add(agenda);
                    }                         
                }                    
                break;
            case DayOfWeek.Wednesday:
                agenda.diaDaSemana = "Quarta-feira";
                if(oldDoc.diario[2] == null){
                    agenda.id = countId++;
                    oldDoc.diario[2] = new List<Agenda> {agenda};                                       
                }
                else if(oldDoc.diario[2] != null){
                    if(oldDoc.diario[2].Count() == 5){
                        throw new IndexOutOfRangeException();                        
                    }
                    if(agenda.atendimento == 2 && oldDoc.diario[2].Any(agenda => agenda.atendimento == 2)){
                        throw new ArgumentOutOfRangeException();
                    }
                    else{
                        agenda.id = countId++;
                        oldDoc.diario[2].Add(agenda);
                    }                            
                }
                break;
            case DayOfWeek.Thursday:
                agenda.diaDaSemana = "Quinta-feira";
                if(oldDoc.diario[3] == null){
                    agenda.id = countId++;
                    oldDoc.diario[3] = new List<Agenda> {agenda};                                         
                }
                else if(oldDoc.diario[3] != null){
                    if(oldDoc.diario[3].Count() == 5){
                        throw new IndexOutOfRangeException();                        
                    }
                    if(agenda.atendimento == 2 && oldDoc.diario[3].Any(agenda => agenda.atendimento == 2)){
                        throw new ArgumentOutOfRangeException();
                    }
                    else{
                        agenda.id = countId++;
                        oldDoc.diario[3].Add(agenda);
                    }                      
                }
                break;
            case DayOfWeek.Friday:
                agenda.diaDaSemana = "Sexta-feira";
                if(oldDoc.diario[4] == null){
                    agenda.id = countId++;
                    oldDoc.diario[4] = new List<Agenda> {agenda};                                              
                }
                else if(oldDoc.diario[4] != null){
                    if(oldDoc.diario[4].Count() == 5){
                        throw new IndexOutOfRangeException();
                        
                    }
                    if(agenda.atendimento == 2 && oldDoc.diario[4].Any(agenda => agenda.atendimento == 2)){
                        throw new ArgumentOutOfRangeException();
                    }
                    else{
                        agenda.id = countId++;
                        oldDoc.diario[4].Add(agenda);
                    }      
                }
                break;
            }
        return true;
    }

    // Cancela um agendamento
    public bool cancel(int CRM, int Id){
        Doutor oldDoc = getDoc(CRM);
        Agenda agendaDoc = getAppointment(CRM, Id);
        if(oldDoc == null){
            throw new ArgumentNullException();
        }
        // Percorre o diario do doutor, ao encontrar a agenda designada, remove do diario.
        foreach(List<Agenda> horario in oldDoc.diario){
            if(horario != null){
                for(var i=0;i<horario.Count;i++){
                    if(horario[i].id == agendaDoc.id){
                        horario.Remove(agendaDoc);
                        return true;
                    }
                }
            }
        }
        throw new ArgumentNullException();
    }
}