using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class LogHandler : Handler
{
    public LogHandler(ILogger logger, DatabaseContext context, IConfiguration configuration)
        : base(logger, context, configuration)
    {
    }

    public ActionResult<List<Log>> GetAllLogs(User? user, int? quantity)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        
        if (user.Type != AccountType.Admin)
        {
            return new BadRequestResult();
        }
        var logs = Services.Logs.GetAll();
        
        return new OkObjectResult(quantity != null ? logs.Take((int)quantity) : logs);
    }
}