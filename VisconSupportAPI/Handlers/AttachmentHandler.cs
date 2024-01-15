using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class AttachmentHandler : Handler
{
    public AttachmentHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    public ActionResult<Attachment> GetAttachment(int attachmentId)
    {
        Attachment? attachment = Services.Attachments.GetById(attachmentId);
        
        if (attachment == null)
        {
            return new NotFoundResult();
        }

        FileStream stream = Services.Attachments.GetFileById(attachment.Id);
        
        return new FileStreamResult(stream, attachment.MimeType);
    }

    public ActionResult AuthenticateAttachment(User? user, int attachmentId, string mimeType)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        
        Attachment? attachment = Services.Attachments.GetById(attachmentId);

        if (attachment == null)
        {
            return new NotFoundResult();
        }

        if (attachment.MimeType != mimeType)
        {
            return new BadRequestResult();
        }

        Issue? selectedIssue = Services.Issues.GetById(attachment.IssueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return new ForbidResult();
        }

        return new OkResult();
    }
}