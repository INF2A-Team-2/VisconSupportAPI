using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/attachments")]
public class AttachmentController : BaseController<AttachmentController>
{
    public AttachmentController(ILogger<AttachmentController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }
    
    [HttpGet("{attachmentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<List<Attachment>> GetAttachment(int attachmentId)
    {
        Attachment? attachment = Context.Attachments.FirstOrDefault(a => a.Id == attachmentId);

        if (attachment == null)
        {
            return NotFound();
        }

        string? ext = Utils.GetFileExtension(attachment.MimeType);

        if (ext == null)
        {
            return StatusCode(500, new
            {
                Message = "Invalid file mimetype"
            });
        }

        string path = Path.Join(Configuration["DataPath"], $"{attachment.Id}{ext}");

        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return File(stream, attachment.MimeType);
    }
    
    [HttpGet("{attachmentId:int}/authenticate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult AuthenticateAttachment(int attachmentId, string mimeType)
    {
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized();
        }
        
        Attachment? attachment = Context.Attachments.FirstOrDefault(a => a.Id == attachmentId);

        if (attachment == null)
        {
            return NotFound();
        }

        if (attachment.MimeType != mimeType)
        {
            return BadRequest();
        }

        Issue? selectedIssue = Context.Issues.FirstOrDefault(i => i.Id == attachment.IssueId);

        if (selectedIssue == null)
        {
            return NotFound();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return Forbid();
        }

        return Ok();
    }
}