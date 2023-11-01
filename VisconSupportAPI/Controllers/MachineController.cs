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
    public ActionResult<List<Machine>> GetMachines(int? userId){
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

        if (userId != null)
        {
            User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);

            if (selectedUser == null)
            {
                return NotFound();
            }
            
            Context.Entry(selectedUser).Collection(u => u.Machines).Load();
            return Ok(selectedUser.Machines);
        }
        
        return Ok(Context.Machines);
    }

    [HttpPut]
    [Authorize]
    [Route("{userId:int}")]
    public ActionResult AddMachine(int userId, List<long> machineIds)
    {
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized("Not authorized");
        }
        
        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);
        
        if (selectedUser == null)
        {
            return NotFound();
        }

        if (selectedUser.Type != AccountType.User)
        {
            return BadRequest();
        }
        
        Context.Entry(selectedUser).Collection(u => u.Machines).Load();

        List<Machine> machines = Context.Machines.Where(m => machineIds.Contains(m.Id)).ToList();
        
        selectedUser.Machines.Clear();
        selectedUser.Machines.AddRange(machines);
        
        Context.SaveChanges();

        return Ok();
    }
}
