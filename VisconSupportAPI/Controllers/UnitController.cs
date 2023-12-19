using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/units")]
public class UnitController : Controller<UnitController, UnitHandler>
{
    public UnitController(ILogger<UnitController> logger, DatabaseContext context, IConfiguration configuration)
        : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<List<Unit>> GetUnits() => Handler.GetAllUnits();

    [HttpGet("{unitId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<Unit> GetUnit(int unitId) => Handler.GetUnitById(unitId);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Unit> PostUnit(NewUnit data) => Handler.CreateUnit(data);

    [HttpPut("{unitId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult PutUnit(int unitId, NewUnit data) => Handler.EditUnit(unitId, data);

    [HttpDelete("{unitId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteUnit(int unitId) => Handler.DeleteUnit(unitId);
}
