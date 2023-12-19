using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : Controller<CompanyController, CompanyHandler>
{
    public CompanyController(ILogger<CompanyController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<List<Company>> GetCompanies() => Handler.GetAllCompanies(GetUserFromClaims());

    [HttpGet("{companyId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<Company> GetCompany(int companyId) => Handler.GetCompanyById(GetUserFromClaims(), companyId);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Company> PostCompany(NewCompany data) => Handler.CreateCompany(GetUserFromClaims(), data);

    [HttpPut("{companyId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult PutCompany(int companyId, NewCompany data) => Handler.EditCompany(GetUserFromClaims(), companyId, data);

    [HttpDelete("{companyId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteCompany(int companyId) => Handler.DeleteCompany(GetUserFromClaims(), companyId);
}