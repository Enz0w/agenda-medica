using AgendaMedica.Models;
namespace AgendaMedica.Service;

public interface IScheduleService
{
    
    bool newDoc(Doutor doc);
    bool deleteDoc(int CRM);
    bool changeAppointment(int CRM, int Id, Agenda agenda);
    bool cancelAppointment(int CRM, int Id);
    List<Doutor> listDocs();
    Doutor getDoc(int CRM);
    Agenda getAppointment(int CRM, int Id);
    bool saveAttendance(int CRM, Agenda agenda);
    bool cancel(int CRM, int Id);
}
