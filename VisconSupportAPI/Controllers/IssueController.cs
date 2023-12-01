using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/issues")]
public class IssueController: Controller<IssueController, IssueHandler>
{
    public IssueController(ILogger<IssueController> logger, DatabaseContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<List<Issue>> GetIssues(int? machineId, int? userId, int? quantity) => Handler.GetAllIssues(GetUserFromClaims(), machineId, userId, quantity);

    [HttpGet("{issueId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Issue> GetIssue(int issueId) => Handler.GetIssueById(GetUserFromClaims(), issueId);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<Issue> CreateIssue(NewIssue ticket, int? userId) => Handler.CreateIssue(GetUserFromClaims(), ticket, userId);

    [HttpGet("{issueId:int}/messages")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<Message>> GetMessages(int issueId) => Handler.GetAllIssueMessages(GetUserFromClaims(), issueId);

    [HttpGet("{issueId:int}/messages/{messageId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Message> GetMessage(int issueId, int messageId) => Handler.GetIssueMessageById(GetUserFromClaims(), issueId, messageId);

    [HttpPost("{issueId:int}/messages")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Message> CreateMessage(int issueId, NewMessage message) => Handler.CreateIssueMessage(GetUserFromClaims(), issueId, message);

    [HttpGet("{issueId:int}/attachments")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<List<Attachment>> GetAttachments(int issueId) => Handler.GetAllIssueAttachments(GetUserFromClaims(), issueId);

    [HttpPost("{issueId:int}/attachments")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<int> UploadAttachment(int issueId, NewAttachment attachment) => Handler.UploadIssueAttachment(GetUserFromClaims(), issueId, attachment);
}