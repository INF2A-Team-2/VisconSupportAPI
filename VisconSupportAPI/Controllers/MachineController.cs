using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<List<Machine>> GetMachines(){
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized("Not authorized");
        }

        if (user.Type == AccountType.User)
        {
            Context.Entry(user).Collection(u => u.Machines).Load();
            return Ok(user.Machines);
        }
        
        return Ok(Context.Machines);
    }
}
