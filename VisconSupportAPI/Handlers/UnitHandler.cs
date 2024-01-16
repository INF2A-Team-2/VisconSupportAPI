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

    public ActionResult<Unit> CreateUnit(User? user, NewUnit data)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.User)
        {
            return new ForbidResult();
        }
        
        Unit createdUnit = Services.Units.Create(data);
        
        Services.Logs.Create(user, $"Unit: \"{createdUnit.Name}\" [{createdUnit.Id}] has been created", unit: createdUnit); 

        
        return new CreatedAtActionResult(
            "GetUnit",
            "Unit",
            new { unitId = createdUnit.Id },
            createdUnit);
    }

    public ActionResult EditUnit(User? user, int unitId, NewUnit data)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.User)
        {
            return new ForbidResult();
        }
        
        Unit? unit = Services.Units.GetById(unitId);

        if (unit == null)
        {
            return new NotFoundResult();
        }
        
        Services.Units.Edit(unitId, data);
        
        Services.Logs.Create(user, $"Report: \"{unit.Name}\" [{unit.Id}] has been edited", unit: unit); 
        
        return new NoContentResult();
    }

    public ActionResult DeleteUnit(User? user, int unitId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.User)
        {
            return new ForbidResult();
        }

        Unit? unit = Services.Units.GetById(unitId);

        if (unit == null)
        {
            return new NotFoundResult();
        }
        
        Services.DetachEntity(unit);
        
        Services.Units.Delete(unitId);
        
        Services.Logs.Create(user, $"Report: \"{unit.Name}\" [{unit.Id}] has been deleted", unit: unit); 
        
        return new OkResult();
    }
}
