using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class IssueHandler : Handler
{
    public IssueHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }
    
    public ActionResult<List<Issue>> GetAllIssues(User? user, int? machineId, int? userId, int? quantity)
    {
        if(user == null)
        {
            return new UnauthorizedResult();
        }

        List<Issue> issues = new List<Issue>();
        
        switch (user.Type)
        {
            case AccountType.User:
                issues = Context.Issues.Where(i => i.UserId == user.Id 
                                                   && (i.Status == Status.Open || i.Status == Status.InProgress)).ToList();
                break;
            
            case AccountType.Helpdesk:
                issues = (from issue in Services.Issues.GetAll()
                    join newUser in Services.Users.GetAll() on issue.UserId equals newUser.Id
                    where newUser.UnitId == user.UnitId && issue.Status is Status.Open or Status.InProgress
                    select issue).ToList();
                break;
            
            case AccountType.Admin:
                if (userId != null)
                {
                    issues = issues.Where(i => i.UserId == userId).ToList();
                    break;
                }
                issues = Services.Issues.GetAll();
                break;
            
            default:
                return new BadRequestResult();
        }
        
        if (machineId != null)
        {
            issues = issues.Where(i => i.MachineId == machineId).ToList();
        }

        return new OkObjectResult(quantity != null ? issues.Take((int)quantity) : issues);
    }
    
    public ActionResult<Issue> GetIssueById(User? user, int issueId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Issue? selectedIssue = Services.Issues.GetById(issueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(selectedIssue);
    }

    public ActionResult<Issue> CreateIssue(User? user, NewIssue ticket, int? userId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        User issueUser = user;

        if (user.Type != AccountType.User && userId != null)
        {
            User? selectedUser = Services.Users.GetById(userId.Value);

            if (selectedUser == null)
            {
                return new NotFoundResult();
            }

            issueUser = selectedUser;
        }

        Issue issue = Services.Issues.Create(ticket, issueUser);

        Services.Logs.Create(user, $"Ticket: \"{issue.Headline}\" [{issue.Id}] has been created", issue: issue); 
        
        return new CreatedAtActionResult(
        "GetIssue",
        "Issue",
        new { issueId = issue.Id },
        issue);
    }

    public ActionResult ChangeIssuePriority(User? user, Priority priority, int issueId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type is AccountType.User) return new ForbidResult();

        Issue? issue = Services.Issues.GetById(issueId);

        if (issue == null)
        {
            return new BadRequestResult();
        }
        
        issue.Priority = priority;
        
        Services.Logs.Create(user, $"Ticket: \"{issue.Headline}\" [{issue.Id}] priority has been set to \"{priority}\"", issue: issue);
        
        return Services.Issues.Edit(issueId, issue, user) ? new OkResult() : new BadRequestResult();
    }
    
    public ActionResult<List<Message>> GetAllIssueMessages(User? user, int issueId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Issue? selectedIssue = Services.Issues.GetById(issueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        Services.LoadCollection(selectedIssue, i => i.Messages);
        
        return new OkObjectResult(selectedIssue.Messages);
    }

    public ActionResult<Message> GetIssueMessageById(User? user, int issueId, int messageId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Issue? selectedIssue = Services.Issues.GetById(issueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        Message? selectedMessage = Services.Messages.GetById(messageId);

        if (selectedMessage == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(selectedMessage);
    }

    public ActionResult<Message> CreateIssueMessage(User? user, int issueId, NewMessage message)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Issue? selectedIssue = Services.Issues.GetById(issueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return new ForbidResult();
        }

        Services.LoadReference(selectedIssue, i => i.User);
        
        if (selectedIssue.Status == Status.Open)
        {
            selectedIssue.Status = Status.InProgress;
            Services.Issues.Edit(selectedIssue.Id, selectedIssue, selectedIssue.User);
        }

        Message createdMessage = Services.Messages.Create(message, selectedIssue, user);
        
        return new CreatedAtActionResult(
            "GetMessage",
            "Issue",
            new { issueId = createdMessage.IssueId, messageId = createdMessage.Id },
            createdMessage);

    }

    public ActionResult<List<Attachment>> GetAllIssueAttachments(User? user, int issueId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Issue? selectedIssue = Services.Issues.GetById(issueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return new ForbidResult();
        }

        return new OkObjectResult(Context.Attachments.Where(a => a.IssueId == issueId));
    }
    
    public ActionResult<int> UploadIssueAttachment(User? user, int issueId, NewAttachment attachment)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Issue? selectedIssue = Context.Issues.FirstOrDefault(i => i.Id == issueId);

        if (selectedIssue == null)
        {
            return new NotFoundResult();
        }

        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk) && selectedIssue.UserId != user.Id)
        {
            return new ForbidResult();
        }

        Attachment newAttachment = Services.Attachments.Create(attachment, selectedIssue);

        return new OkObjectResult(newAttachment);
    }

    public ActionResult ResolveIssue(User? user, int issueId, NewReport report)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        if (user.Type is not (AccountType.Admin or AccountType.Helpdesk))
        {
            return new ForbidResult();
        }

        Issue? issue = Services.Issues.GetById(issueId);

        if (issue == null)
        {
            return new NotFoundResult();
        }
        
        issue.Status = Status.Archived;

        report.IssueId = issue.Id;
        report.MachineId = issue.MachineId;
        
        Services.Reports.Create(report);
        
        Services.Logs.Create(user, $"Ticket: \"{issue.Headline}\" [{issue.Id}] has been resolved", issue: issue); 
        
        return new OkResult();
    }
}