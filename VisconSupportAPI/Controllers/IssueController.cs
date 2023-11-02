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
        
        return Created(
            Url.Action("GetIssue", "Issue", new { issueId=issue.Id}, Request.Scheme) ?? "",
            issue);
    }
    
    [HttpGet("{issueId:int}/messages")]
    [Authorize]
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
                Timestamp = message.TimeStamp
            });
        }

        return Ok(retour);
    }

    [HttpPost("{issueId:int}/messages")]
    [Authorize]
    public ActionResult CreateMessage(int issueId, NewMessage message)
    {
        User? user = GetUserFromClaims();
        if (user == null)
            return Unauthorized();

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) &&
            Context.Issues.First(h => h.Id == issueId).UserId != user.Id) return BadRequest();
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
}