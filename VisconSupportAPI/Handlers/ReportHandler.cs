using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class ReportHandler : Handler
{
    public ReportHandler(ILogger logger, DatabaseContext context, IConfiguration configuration)
        : base(logger, context, configuration)
    {
    }

    public ActionResult<List<Report>> GetAllReports(User? user)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.Admin)
            return new OkObjectResult(Services.Reports.GetAll());
        if (user.Type == AccountType.Helpdesk)
            return new OkObjectResult(Services.Reports.GetAll());
        if (user.Type == AccountType.User)
        {
            Services.LoadReference(user, u => u.Company);
            if (user.CompanyId == null || user.Company == null)
                return new BadRequestResult();
            var company = user.Company;
            Services.LoadCollection(company, c => c.Machines);
            return new OkObjectResult(Services.Reports.GetAll()
                .Where(h => user.CompanyId == h.CompanyId || (company.Machines.Contains(h.Machine) && h.Public)));
        }
        return new BadRequestResult();
    }

    public ActionResult<Report> GetReportById(int reportId)
        {
            Report? report = Services.Reports.GetById(reportId);
        
            if (report == null)
            {
                return new NotFoundResult();
            }
        
            return new OkObjectResult(report);
        }

        public ActionResult<Report> CreateReport(User? user, NewReport data)
        {
            if(user == null) return new UnauthorizedResult();
            if(user.Type is not AccountType.Admin and AccountType.Helpdesk) return new ForbidResult();
            Report? createdReport = Services.Reports.Create(data);
            if(createdReport == null) return new BadRequestResult();
        
            return new CreatedAtActionResult(
                "GetReport",
                "Report",
                new { reportId = createdReport.Id },
                createdReport);
        }

        public ActionResult EditReport(User? user, int reportId, NewReport data)
        {
            if(user == null) return new UnauthorizedResult();
            if(user.Type is not AccountType.Admin and AccountType.Helpdesk) return new ForbidResult();
            bool isEdited = Services.Reports.Edit(reportId, data);
        
            if (!isEdited)
            {
                return new NotFoundResult();
            }
        
            return new NoContentResult();
        }

        public ActionResult DeleteReport(User? user, int reportId)
        {
            if(user == null) return new UnauthorizedResult();
            if(user.Type is not AccountType.Admin and AccountType.Helpdesk) return new ForbidResult();
            bool isDeleted = Services.Reports.Delete(reportId);
        
            if (!isDeleted)
            {
                return new NotFoundResult();
            }
        
            return new OkResult();
        }
}
