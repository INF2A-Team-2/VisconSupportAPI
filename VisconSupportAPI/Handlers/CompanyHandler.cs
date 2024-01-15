using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class CompanyHandler : Handler
{
    public CompanyHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    public ActionResult<List<Company>> GetAllCompanies(User? user)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        return new OkObjectResult(Services.Companies.GetAll());
    }
    
    public ActionResult<Company> GetCompanyById(User? user, int companyId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        
        Company? retrievedUser = Services.Companies.GetById(companyId);

        return new OkObjectResult(retrievedUser);
    }

    public ActionResult<Company> CreateCompany(User? user, NewCompany data)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        Company createdCompany = Services.Companies.Create(data);
        
        Services.Logs.Create(user, $"Company: \"{createdCompany.Name}\" [{createdCompany.Id}] has been created", company: createdCompany); 

        return new CreatedAtActionResult(
            "GetCompany",
            "Company",
            new { companyId = createdCompany.Id },
            createdCompany);
    }

    public ActionResult EditCompany(User? user, int companyId, NewCompany data)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        Company? company = Services.Companies.GetById(companyId);

        if (company == null)
        {
            return new NotFoundResult();
        }

        Services.Companies.Edit(companyId, data);

        Services.Logs.Create(user, $"Company: \"{company.Name}\" [{company.Id}] has been edited", company: company);
        
        return new NoContentResult();
    }
    
    public ActionResult DeleteCompany(User? user, int companyId)
    {   
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }
        
        Company? company = Services.Companies.GetById(companyId);

        if (company == null)
        {
            return new NotFoundResult();
        }
        
        Services.DetachEntity(company);
        
        Services.Companies.Delete(companyId);
        
        Services.Logs.Create(user, $"Company: \"{company.Name}\" [{company.Id}] has been deleted", company: company);

        return new OkResult();
    }
}