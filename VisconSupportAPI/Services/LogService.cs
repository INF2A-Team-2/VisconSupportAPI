using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class LogService : Service
{
    public LogService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context,
        configuration, services)
    {
    }
    
    public Log? GetById(int id) => Context.Logs.FirstOrDefault(i => i.Id == id);
    
    public List<Log> GetAll() => Context.Logs.OrderBy(x => x.TimeStamp).ToList();

    public Log Create(User author, string description, 
        Issue? issue = null,
        User? user = null,
        Machine? machine = null,
        Message? message = null,
        Attachment? attachment = null)
    {
        Log log = new Log()
        {
            TimeStamp = DateTime.UtcNow,
            AuthorId = author.Id,
            Description = description,
            IssueId = issue?.Id,
            UserId = user?.Id,
            MachineId = machine?.Id,
            MessageId = message?.Id,
            AttachmentId = attachment?.Id
        };

        Context.Logs.Add(log);
        Context.SaveChanges();

        return log;
    }
}