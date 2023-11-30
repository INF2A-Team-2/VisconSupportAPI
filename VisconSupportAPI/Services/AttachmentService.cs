using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class AttachmentService : Service
{
    public AttachmentService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Attachment? GetById(int id) => Context.Attachments.FirstOrDefault(a => a.Id == id);

    public List<Attachment> GetAll() => Context.Attachments.ToList();

    public FileStream GetFileById(int id)
    {
        Attachment? attachment = GetById(id);
        
        if (attachment == null)
        {
            throw new ArgumentException($"Attachment with ID {id} not found", nameof(id));
        }

        string path = Path.Join(Configuration["DataPath"], $"{attachment.Id}.{attachment.Name?.Split(".")[^1]}");

        if (!Path.Exists(path))
        {
            throw new FileNotFoundException($"Attachment file with ID {id} not found");
        }

        return new FileStream(path, FileMode.Open, FileAccess.Read);
    }

    public Attachment Create(NewAttachment data, Issue issue)
    {
        Attachment attachment = new Attachment()
        {
            Name = data.Name,
            MimeType = data.MimeType,
            IssueId = issue.Id
        };

        Context.Attachments.Add(attachment);

        Context.SaveChanges();

        return attachment;
    }

    public void Edit(int id, NewAttachment data, Issue issue)
    {
        Attachment? attachment = GetById(id);

        if (attachment == null)
        {
            throw new ArgumentException($"Attachment with ID {id} not found", nameof(id));
        }

        attachment.Name = data.Name;
        attachment.MimeType = data.MimeType;
        attachment.IssueId = issue.Id;

        Context.SaveChanges();
    }

    public void Delete(int id)
    {
        Attachment? attachment = GetById(id);

        if (attachment != null)
        {
            Context.Attachments.Remove(attachment);
            Context.SaveChanges();
        }
        
    }
}