using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/machines/import")]
public class ImportFileController: BaseController
{
    public ImportFileController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }

    [HttpPost]
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
                if(Context.Machines.Any(h => h.Name == machine.Name)) continue;
                currUser.Machines.Add(new Machine() { Name = machine.Name });
            }
        }

        return Ok();
    }

    // [HttpGet]
    // [Authorize]
    // public ActionResult GetExample()
    // {
    //     var records = Context.Users.First(h => h.Id == 1);
    //     Context.Entry(records).Collection(h => h.Machines).Load();
    //     using (var writer = new StreamWriter("file.csv"))
    //     using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    //     {
    //         csv.WriteRecords(records.Machines);
    //     }
    //
    //     return Ok();
    // }
}

public class ReadMachine
{
    public long UserId { get; set; }
    public string Name { get; set; }
}