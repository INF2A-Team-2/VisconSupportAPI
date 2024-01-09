using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;
namespace VisconSupportAPI.Services;

public class ReportService : Service
{
    public ReportService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Report? GetById(int id) => Context.Reports.FirstOrDefault(r => r.Id == id);

    public List<Report> GetAll() => Context.Reports.ToList();

    public Report Create(NewReport data)
    {
        Report report = new Report()
        {
            Title = data.Title,
            Body = data.Body,
            Public = data.pub, 
            MachineId = data.MachineId,
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
        report.Public = data.pub;
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
