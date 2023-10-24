using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected readonly ILogger<AuthController> Logger;

    protected readonly DatabaseContext Context;

    protected readonly IConfiguration Configuration;
    
    public BaseController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration)
    {
        Logger = logger;
        Context = context;
        Configuration = configuration;
    }
    
    protected User? GetUserFromClaims()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return null;
        }

        User? user = Context.Users.FirstOrDefault(h => h.Username == userId);

        return user;
    }

}