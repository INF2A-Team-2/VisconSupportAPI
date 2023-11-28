using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using CsvHelper;

[ApiController]
[Route("api/machines")]

public class MachineController : BaseController<MachineController>
{
    public MachineController(ILogger<MachineController> logger, DatabaseContext context, IConfiguration configuration) 
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
    
    [HttpPost("import")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult ImportMachines([FromForm]IFormFile formFile)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        if (formFile.Length <= 0 || !formFile.FileName.EndsWith(".csv")) return BadRequest();

        using (var reader = new StreamReader(formFile.OpenReadStream())) 
        {
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var readMachines = csv.GetRecords<ReadMachine>().ToList();
            if (!readMachines.Any()) return NoContent();

            foreach (var machine in readMachines)
            {
                var currUser = Context.Users.FirstOrDefault(h => h.Id == machine.UserId);
                if(currUser == null) continue;
                Context.Entry(currUser).Collection(h => h.Machines).Load();
                var exists = Context.Machines.FirstOrDefault(h => h.Name == machine.Name);
                currUser.Machines.Add(exists ?? new Machine() { Name = machine.Name });
            }

            Context.SaveChanges();
        }

        return Ok();
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult AddMachine(newMachine machine)
    {
        User? user = GetUserFromClaims();
        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }
        
        var currUser = Context.Users.FirstOrDefault(h => h.Id == machine.userId);
        if(currUser == null) return BadRequest();
        Context.Entry(currUser).Collection(h => h.Machines).Load();
        var exists = Context.Machines.FirstOrDefault(h => h.Name == machine.machineName);
        currUser.Machines.Add(exists ?? new Machine() { Name = machine.machineName });
        var quant = Context.SaveChanges();
        return quant > 0 ? Ok() : BadRequest();
    }
}

public class ReadMachine
{
    public long UserId { get; set; }
    public string Name { get; set; }
    
}

public class newMachine
{
    public long userId { get; set; }
    
    public string machineName { get; set; }
}
