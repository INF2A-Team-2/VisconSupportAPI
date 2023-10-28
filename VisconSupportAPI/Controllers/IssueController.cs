using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    
    [HttpGet("{machineId}")]
    [Authorize]
    public ActionResult<List<Issue>> GetIssues(string machineId){
        User? user = GetUserFromClaims();
        if(user == null)
        {
            return Unauthorized();
        }

        if(int.TryParse(machineId, out var machine)){
            return Ok(Context.Issues.Where(h => h.UserId == user.Id && h.MachineId == machine));
        }
        return BadRequest();
    }

    [HttpPut()]
    [Authorize]

    public ActionResult<Issue> CreateIssue([FromBody] NewIssue Ticket)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        Context.Issues.Add(new Issue()
        {
            Actual = Ticket.Actual,
            Expected = Ticket.Expected,
            Tried = Ticket.Tried,
            Headline = Ticket.Headline,
            UserId = user.Id,
            MachineId = Ticket.MachineId,
            TimeStamp = DateTime.UtcNow
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