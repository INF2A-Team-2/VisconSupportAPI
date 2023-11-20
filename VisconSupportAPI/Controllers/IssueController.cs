using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Logic;
using VisconSupportAPI.Models;
using System;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/issues")]
public class IssueController: BaseController
{
    public readonly string[] AllowedAttachmentTypes = new []
    {
        "image",
        "video"
    };
    
    public IssueController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<List<Issue>> GetIssues(int? machineId, int? userId, int? quantity)
    {
        User? user = GetUserFromClaims();
        
        if(user == null)
        {
            return Unauthorized();
        }

        IEnumerable<Issue> issues = new List<Issue>();
        
        switch (user.Type)
        {
            case AccountType.User:
                issues = Context.Issues.Where(i => i.UserId == user.Id);
                break;
            case AccountType.Helpdesk:
                issues = from issue in Context.Issues
                    join newUser in Context.Users on issue.UserId equals newUser.Id
                    where newUser.Unit == user.Unit
                    select issue;
                break;
            case AccountType.Admin:
                if (userId != null)
                {
                    issues = issues.Where(i => i.UserId == userId);
                    break;
                }
                issues = Context.Issues;
                break;
            default:
                return BadRequest();
        }
        
        if (machineId != null)
        {
            issues = issues.Where(i => i.MachineId == machineId);
        }

        return Ok(quantity != null ? issues.Take((int)quantity) : issues);
    }

    [HttpGet("{issueId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public ActionResult<Issue> GetIssue(int issueId)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        Issue? selectedIssue = Context.Issues.FirstOrDefault(h => h.Id == issueId);

        if (selectedIssue == null)
        {
            return NotFound();
        }

        return Ok(selectedIssue);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<Issue> CreateIssue([FromBody] NewIssue Ticket, int? userId)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        int? newUser = null;

        if (user.Type == AccountType.Helpdesk && userId != null)
            newUser = (int)userId;

        Issue issue = new Issue()
        {
            Actual = Ticket.Actual,
            Expected = Ticket.Expected,
            Tried = Ticket.Tried,
            Headline = Ticket.Headline,
            UserId = newUser ?? user.Id,
            MachineId = Ticket.MachineId,
            TimeStamp = DateTime.UtcNow
        };
        
        Context.Issues.Add(issue);
        Context.SaveChanges();
        
        return Created(
            Url.Action("GetIssue", "Issue", new { issueId=issue.Id}, Request.Scheme) ?? "",
            issue);
    }
    
    [HttpGet("{issueId:int}/messages")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<List<Message>> GetMessages(int issueId)
    {
        User? user = GetUserFromClaims();
        if (user == null)
            return Unauthorized();

        var retour = MessageLogic.getMessages(user, issueId);
        if (retour == null) return NotFound();

        return Ok(retour);
    }

    [HttpPost("{issueId:int}/messages")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult CreateMessage(int issueId, NewMessage message)
    {
        User? user = GetUserFromClaims();
        if (user == null)
            return Unauthorized();

        Issue? selectedIssue = Context.Issues.FirstOrDefault(i => i.Id == issueId);

        if (selectedIssue == null)
        {
            return NotFound();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return Forbid();
        }
        
        Context.Messages.Add(new Message
        {
            Body = message.Body,
            TimeStamp = DateTime.UtcNow,
            IssueId = issueId,
            UserId = user.Id
        });
        Context.SaveChanges();
        return Ok();

    }

    [HttpGet("{issueId:int}/attachments")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<List<RetourAttachment>> GetAttachments(int issueId)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        Issue? selectedIssue = Context.Issues.FirstOrDefault(i => i.Id == issueId);

        if (selectedIssue == null)
        {
            return NotFound();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return Forbid();
        }
        
        return Ok(Context.Attachments.Where(a => a.IssueId == selectedIssue.Id).Select(a => new RetourAttachment()
        {
            ID = a.Id,
            MimeType = a.MimeType,
            Chunks = a.Chunks.Where(c => c.AttachmentID == a.Id).Select(c => c.Id).ToList()
        }));
    }

    [HttpPost("{issueId:int}/attachments")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Attachment> CreateAttachment(int issueId, NewAttachment data)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        Issue? selectedIssue = Context.Issues.FirstOrDefault(i => i.Id == issueId);

        if (selectedIssue == null)
        {
            return NotFound();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return Forbid();
        }
        
        if (!AllowedAttachmentTypes.Contains(data.MimeType.Split("/")[0]))
        {
            return BadRequest();
        }

        Attachment attachment = new Attachment()
        {
            IssueId = issueId,
            MimeType = data.MimeType
        };

        Context.Attachments.Add(attachment);
        Context.SaveChanges();
        
        return Ok(attachment);
    }

    [HttpPost("{issueId:int}/attachments/{attachmentId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UploadChunk(int issueId, int attachmentId)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        Issue? selectedIssue = Context.Issues.FirstOrDefault(i => i.Id == issueId);

        if (selectedIssue == null)
        {
            return NotFound();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return Forbid();
        }

        Attachment? attachment = Context.Attachments.FirstOrDefault(a => a.Id == attachmentId);

        if (attachment == null)
        {
            return NotFound();
        }
        
        byte[] data;
        using (MemoryStream ms = new MemoryStream())
        {
            await Request.Body.CopyToAsync(ms);
            data = ms.ToArray();
        }

        List<FileChunk> existingChunks = Context.FileChunks.Where(c => c.AttachmentID == attachment.Id).ToList();

        FileChunk chunk = new FileChunk()
        {
            AttachmentID = attachment.Id,
            ChunkNumber = existingChunks.Count,
            Data = data
        };

        Context.FileChunks.Add(chunk);
        await Context.SaveChangesAsync();

        return Ok();
    }
}

public class NewAttachment
{
    public string MimeType { get; set; }
}

public class RetourAttachment
{
    public long ID { get; set; }
    public string MimeType { get; set; }
    public List<long> Chunks { get; set; }
}

public class NewIssue
{
    public string Actual { get; set; }
    public string Expected { get; set; }
    public string Tried { get; set; }
    public string Headline { get; set; }
    public long MachineId { get; set; }
}

public class NewMessage
{
    public string Body { get; set; }
}

public class RetourMessage
{
    public long ID { get; set; }
    public string Name { get; set; }
    public string Body { get; set; }
    public DateTime Timestamp { get; set; }
    public long UserID { get; set; }
}