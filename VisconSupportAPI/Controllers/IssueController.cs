using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/issues")]
public class IssueController: BaseController
{
    public IssueController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<List<Issue>> GetIssues(int? machineId, int? userId)
    {
        User? user = GetUserFromClaims();
        
        if(user == null)
        {
            return Unauthorized();
        }
        
        IEnumerable<Issue> issues = user.Type == AccountType.User
            ? Context.Issues.Where(i => i.UserId == user.Id)
            : Context.Issues;

        if (machineId != null)
        {
            issues = issues.Where(i => i.MachineId == machineId);
        }

        if (userId != null)
        {
            issues = issues.Where(i => i.UserId == userId);
        }

        return Ok(issues);
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
    public ActionResult<Issue> CreateIssue([FromBody] NewIssue Ticket)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        Issue issue = new Issue()
        {
            Actual = Ticket.Actual,
            Expected = Ticket.Expected,
            Tried = Ticket.Tried,
            Headline = Ticket.Headline,
            UserId = user.Id,
            MachineId = Ticket.MachineId,
            TimeStamp = DateTime.UtcNow
        };
        
        Context.Issues.Add(issue);
        Context.SaveChanges();
        
        Ticket.Attachments.ForEach(a =>
        {
            string mimeType = a
                .Split(",")[0]
                .Split(";")[0]
                .Split(":")[1];
            
            Context.Attachments.Add(new Attachment()
            {
                Data = a,
                MimeType = mimeType,
                IssueId = issue.Id
            });
        });
        
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

        var retour = new List<RetourMessage>();
        var messages = Context.Messages.Where(h => h.IssueId == issueId).ToList();
        foreach (var message in messages)
        {
            retour.Add(new RetourMessage
            {
                ID = message.Id,
                Name = Context.Users.First(h => h.Id == message.UserId).Username,
                Body = message.Body,
                Timestamp = message.TimeStamp,
                UserID = message.UserId
            });
        }

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
    public ActionResult<List<Attachment>> GetAttachments(int issueId)
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

        return Ok(Context.Attachments.Where(a => a.IssueId == issueId));
    }
}

public class NewIssue
{
    public string Actual { get; set; }
    public string Expected { get; set; }
    public string Tried { get; set; }
    public string Headline { get; set; }
    public long MachineId { get; set; }
    public List<string> Attachments { get; set; }
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