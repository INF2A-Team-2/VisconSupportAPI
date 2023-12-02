using VisconSupportAPI.Data;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class LogService : Service
{
    public LogService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context,
        configuration, services)
    {
        
    }
    
    public Log? GetById(int id) => Context.Logs.FirstOrDefault(i => i.Id == id);
    
    public List<Log> GetAll() => Context.Logs.ToList();

    public Log LoggingMachine(Machine machine)
    {
        
    }
}