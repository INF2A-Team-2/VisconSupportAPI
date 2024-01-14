using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Attributes;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;


[ApiController]
[Route("api/logs")]
public class LogController : Controller<LogController, LogHandler>
{
    public LogController(ILogger<LogController> logger, DatabaseContext context, IConfiguration configuration) : base(logger, context, configuration)
    {
    }   
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [QueryFiltered("Id.asc")]
    public ActionResult<List<Log>> GetLogs(int? quantity) => Handler.GetAllLogs(GetUserFromClaims(), quantity);
}