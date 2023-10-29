using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController: BaseController
{
    public MessageController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }
    
    [HttpGet]
    [Authorize]
    public ActionResult<List<Message>> GetMessages(int issueId)
    {
        User? user = GetUserFromClaims();
        if (user == null)
            return Unauthorized();

        var retour = new List<RetourMessage>();
        var messages = Context.Messages.Where(h => h.IssueId == issueId && h.UserId == user.Id).ToList();
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

    [HttpPost]
    [Authorize]
    public ActionResult CreateMessage([FromBody] NewMessage message)
    {
        User? user = GetUserFromClaims();
        if (user == null)
            return Unauthorized();

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) &&
            Context.Issues.First(h => h.Id == message.IssueId).UserId != user.Id) return BadRequest();
        Context.Messages.Add(new Message
        {
            Body = message.Body,
            TimeStamp = DateTime.UtcNow,
            IssueId = message.IssueId,
            UserId = user.Id
        });
        Context.SaveChanges();
        return Ok();

    }
}

public class NewMessage
{
    public int IssueId { get; set; }
    public string Body { get; set; }
}

public class RetourMessage
{
    public long ID { get; set; }
    public string Name { get; set; }
    public string Body { get; set; }
    public DateTime Timestamp { get; set; }
}