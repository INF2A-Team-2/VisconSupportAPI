using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Services;

public abstract class Service
{
    protected readonly ServicesList Services;
    
    protected readonly DatabaseContext Context;

    protected readonly IConfiguration Configuration;

    protected Service(DatabaseContext context, IConfiguration configuration, ServicesList services)
    {
        Context = context;
        Configuration = configuration;
        Services = services;
    }
}