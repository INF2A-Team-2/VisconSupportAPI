using System.Reflection.PortableExecutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

[ApiController]
[Route("api/machines")]

public class MachineController : BaseController
{
    public MachineController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    public ActionResult<List<Machine>> GetMachines(){
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user.Machines);
    }

    [HttpGet("issues")]
    [Authorize]
    public ActionResult<List<Issue>> GetIssueByMachine(long machineId){
        User? user = GetUserFromClaims();
        if(user == null)
        {
            return Unauthorized();
        }

        return new ActionResult<List<Issue>>(new AcceptedResult());
    }

}


