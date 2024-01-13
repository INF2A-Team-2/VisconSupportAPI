using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class PagingHandler : Handler
{
    public PagingHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    public ActionResult<int> GetPages(string table, int pageSize)
    {
        if (pageSize <= 0)
        {
            return new BadRequestObjectResult("Page size must be a positive integer");
        }
        
        int count = table.ToLower() switch
        {
            "issues" => Context.Issues.Count(),
            "users" => Context.Users.Count(),
            "companies" => Context.Companies.Count(),
            "units" => Context.Units.Count(),
            "machines" => Context.Machines.Count(),
            "reports" => Context.Reports.Count(),
            _ => -1
        };

        if (count == -1)
        {
            return new NotFoundResult();
        }

        int pages = (int)Math.Ceiling((double)count / pageSize);
        
        return new OkObjectResult(pages);
    }
}