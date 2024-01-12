using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;
using VisconSupportAPI.Services;

namespace VisconSupportAPI.Handlers;

public class ReportHandler : Handler
{
    public ReportHandler(ILogger logger, DatabaseContext context, IConfiguration configuration)
        : base(logger, context, configuration)
    {
    }

    public ActionResult<List<Report>> GetAllReports()
    {
        return new OkObjectResult(Services.Reports.GetAll());
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

    public ActionResult<Report> CreateReport(NewReport data)
    {
        Report createdReport = Services.Reports.Create(data);
        
        return new CreatedAtActionResult(
            "GetReport",
            "Report",
            new { reportId = createdReport.Id },
            createdReport);
    }

    public ActionResult EditReport(int reportId, NewReport data)
    {
        bool isEdited = Services.Reports.Edit(reportId, data);
        
        if (!isEdited)
        {
            return new NotFoundResult();
        }
        
        return new NoContentResult();
    }

    public ActionResult DeleteReport(int reportId)
    {
        bool isDeleted = Services.Reports.Delete(reportId);
        
        if (!isDeleted)
        {
            return new NotFoundResult();
        }
        
        return new OkResult();
    }
}