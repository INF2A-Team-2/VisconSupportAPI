using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class ReportService : Service
{
    public ReportService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Report? GetById(int id) => Context.Reports.FirstOrDefault(r => r.Id == id);

    public List<Report> GetAll() => Context.Reports.ToList();

    public Report? Create(NewReport data)
    {
        var userId = Services.Issues.GetById(data.IssueId).UserId;
        var companyId = Services.Users.GetById(userId).CompanyId;
        if (companyId == null) return null;
        
        Report report = new Report()
        {
            Title = data.Title,
            Body = data.Body,
            Public = data.Public, 
            MachineId = data.MachineId,
            CompanyId = companyId.Value,
            TimeStamp = DateTime.UtcNow
        };

        Context.Reports.Add(report);
        Context.SaveChanges();

        return report;
    }

    public Boolean Edit(int id, NewReport data)
    {
        Report? report = GetById(id);

        if (report == null)
        {
            return false;
        }

        report.Title = data.Title;
        report.Body = data.Body;
        report.Public = data.Public;
        report.MachineId = data.MachineId;

        Context.SaveChanges();
        
        return true;
    }

    public bool Delete(int id)
    {
        Report? report = GetById(id);

        if (report == null)
        {
            return false;
        }
        
        Context.Reports.Remove(report);
        Context.SaveChanges();
        
        return true;

    }
}
