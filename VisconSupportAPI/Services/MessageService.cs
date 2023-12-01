using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class MessageService : Service
{
    public MessageService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Message? GetById(int id) => Context.Messages.FirstOrDefault(i => i.Id == id);

    public List<Message> GetAll() => Context.Messages.ToList();

    public Message Create(NewMessage data, Issue issue, User user)
    {
        Message message = new Message()
        {
            Body = data.Body,
            TimeStamp = DateTime.UtcNow,
            IssueId = issue.Id,
            UserId = user.Id
        };

        Context.Messages.Add(message);
        
        Context.SaveChanges();

        return message;
    }

    public void Edit(int id, NewMessage data, Issue issue, User user)
    {
        Message? message = GetById(id);

        if (message == null)
        {
            throw new ArgumentException($"Issue with ID {id} not found", nameof(id));
        }

        message.Body = data.Body;
        message.IssueId = issue.Id;
        message.UserId = user.Id;

        Context.SaveChanges();
    }

    public void Delete(int id)
    {
        Message? message = GetById(id);

        if (message != null)
        {
            Context.Messages.Remove(message);
            Context.SaveChanges();
        }
    }
}