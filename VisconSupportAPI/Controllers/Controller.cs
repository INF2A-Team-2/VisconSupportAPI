using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
public abstract class Controller<T, THandler> : ControllerBase
{
    protected readonly ILogger Logger;

    protected readonly DatabaseContext Context;

    protected readonly IConfiguration Configuration;

    protected readonly THandler Handler;
    
    protected Controller(ILogger<T> logger, DatabaseContext context, IConfiguration configuration)
    {
        Logger = logger;
        Context = context;
        Configuration = configuration;
        Handler = (THandler)Activator.CreateInstance(typeof(THandler), logger, context, configuration)!;
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