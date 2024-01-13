using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/pages")]
public class PagingController : Controller<PagingController, PagingHandler>
{
    public PagingController(ILogger<PagingController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<int> GetAttachment(string table, int pageSize) => Handler.GetPages(table, pageSize);
}