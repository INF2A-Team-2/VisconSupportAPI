using System.Security.Claims;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;

namespace VisconSupportAPI.Handlers;

public abstract class Handler
{
    protected readonly ILogger Logger;

    protected readonly DatabaseContext Context;

    protected readonly IConfiguration Configuration;

    protected readonly ServicesList Services;

    protected Handler(ILogger logger, DatabaseContext context, IConfiguration configuration)
    {
        Logger = logger;
        Context = context;
        Configuration = configuration;
        Services = new ServicesList(context, configuration);
    }
}