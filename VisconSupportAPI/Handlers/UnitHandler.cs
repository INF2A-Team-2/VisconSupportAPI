using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;
using VisconSupportAPI.Services;

namespace VisconSupportAPI.Handlers;

public class UnitHandler : Handler
{
    public UnitHandler(ILogger logger, DatabaseContext context, IConfiguration configuration)
        : base(logger, context, configuration)
    {
    }

    public ActionResult<List<Unit>> GetAllUnits()
    {
        return new OkObjectResult(Services.Units.GetAll());
    }

    public ActionResult<Unit> GetUnitById(int unitId)
    {
        Unit? unit = Services.Units.GetById(unitId);
        if (unit == null)
        {
            return new NotFoundResult();
        }
        return new OkObjectResult(unit);
    }

    public ActionResult<Unit> CreateUnit(NewUnit data)
    {
        Unit createdUnit = Services.Units.Create(data);
        return new CreatedAtActionResult(
            "GetUnit",
            "Unit",
            new { unitId = createdUnit.Id },
            createdUnit);
    }

    public ActionResult EditUnit(int unitId, NewUnit data)
    {
        bool isEdited = Services.Units.Edit(unitId, data);
        if (!isEdited)
        {
            return new NotFoundResult();
        }
        return new NoContentResult();
    }

    public ActionResult DeleteUnit(int unitId)
    {
        bool isDeleted = Services.Units.Delete(unitId);
        if (!isDeleted)
        {
            return new NotFoundResult();
        }
        return new OkResult();
    }
}
