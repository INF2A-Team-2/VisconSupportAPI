using System.Net;
using System.Net.Mail;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Services;

public class MailService : Service
{   
    private readonly SmtpClient _smtpClient;

    public MailService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base( context, configuration, services)
    {
        _smtpClient = new SmtpClient(
            configuration["MailCredentials:Server"],
            int.Parse(configuration["MailCredentials:Port"] ?? "587"));

        _smtpClient.Credentials = new NetworkCredential(
            configuration["MailCredentials:Email"], 
            configuration["MailCredentials:Password"]);
        
        _smtpClient.EnableSsl = true;
    }

    public void Send(string recipient, string subject, string body)
    {
        MailMessage message = new MailMessage(
            new MailAddress(Configuration["MailCredentials:Email"], Configuration["MailCredentials:DisplayName"]), 
            new MailAddress(recipient));
        
        message.Subject = subject;
        message.Body = body;
        
        _smtpClient.Send(message);
    }
}