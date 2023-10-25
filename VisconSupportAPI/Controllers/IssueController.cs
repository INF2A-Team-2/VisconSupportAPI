using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("issues")]
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
}