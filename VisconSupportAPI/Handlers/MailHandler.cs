using System.Net;
using System.Net.Mail;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Handlers;

public class MailHandler : Handler<MailHandler>
{
    private readonly SmtpClient _smtpClient;
    
    public MailHandler(ILogger<MailHandler> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
        _smtpClient = new SmtpClient(
            configuration["MailCredentials:Server"],
            int.Parse(configuration["MailCredentials:Port"] ?? "587"));

        _smtpClient.Credentials = new NetworkCredential(
            configuration["MailCredentials:Email"], 
            configuration["MailCredentials:Password"]);
        
        _smtpClient.EnableSsl = true;
    }

    public virtual void Send(string recipient, string subject, string body)
    {
        MailMessage message = new MailMessage(
            new MailAddress(Configuration["MailCredentials:Email"], Configuration["MailCredentials:DisplayName"]), 
            new MailAddress(recipient));
        
        message.Subject = subject;
        message.Body = body;
        
        _smtpClient.Send(message);
    }
}