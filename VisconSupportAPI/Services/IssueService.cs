using Microsoft.EntityFrameworkCore;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Services;

public class IssueService : Service
{
    public IssueService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Issue? GetById(int id) => Context.Issues.FirstOrDefault(i => i.Id == id);

    public List<Issue> GetAll() => Context.Issues.ToList();
    
    public Issue Create(NewIssue data, User user)
    {
        Issue issue = new Issue()
        {
            Priority = data.Priority,
            Status = data.Status,
            Actual = data.Actual,
            Expected = data.Expected,
            Tried = data.Tried,
            Headline = data.Headline,
            TimeStamp = DateTime.UtcNow,
            MachineId = data.MachineId,
            UserId = user.Id
        };

        Context.Issues.Add(issue);
        
        Context.SaveChanges();

        return issue;
    }

    public void Edit(int id, NewIssue data, User user)
    {
        Issue? issue = GetById(id);

        if (issue == null)
        {
            throw new ArgumentException($"Issue with ID {id} not found", nameof(id));
        }

        issue.Priority = data.Priority;
        issue.Status = data.Status;
        issue.Actual = data.Actual;
        issue.Expected = data.Expected;
        issue.Tried = data.Tried;
        issue.Headline = data.Headline;
        issue.MachineId = data.MachineId;
        issue.UserId = user.Id;

        Context.SaveChanges();
    }

    public void Delete(int id)
    {
        Issue? issue = GetById(id);

        if (issue != null)
        {
            Context.Issues.Remove(issue);
            Context.SaveChanges();
        }
    }
}