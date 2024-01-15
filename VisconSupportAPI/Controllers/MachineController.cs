using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using CsvHelper;
using VisconSupportAPI.Attributes;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Types;

[ApiController]
[Route("api/machines")]

public class MachineController : Controller<MachineController, MachineHandler>
{
    public MachineController(ILogger<MachineController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [QueryFiltered("Id.asc")]
    public ActionResult<List<Machine>> GetMachines() => Handler.GetAllMachines(GetUserFromClaims());
    
    [HttpGet("{machineId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Machine> GetMachine(int machineId) => Handler.GetMachine(GetUserFromClaims(), machineId);

    // [HttpPost("import")]
    // [Authorize]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    // [ProducesResponseType(StatusCodes.Status204NoContent)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public ActionResult ImportMachines([FromForm] IFormFile formFile) => Handler.ImportMachines(GetUserFromClaims(), formFile);
}