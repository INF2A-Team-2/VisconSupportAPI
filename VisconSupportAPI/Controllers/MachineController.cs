using System.Reflection.PortableExecutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        return new List<Machine> {
            new Machine(0, "machine 1", 2),
            new Machine(1, "machine 2", 2),
            new Machine(2, "machine 3", 2),
            new Machine(3, "machine 4", 2),
            new Machine(4, "machine 5", 2)
        };
        
    }

}


