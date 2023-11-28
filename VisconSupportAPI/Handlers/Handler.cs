using VisconSupportAPI.Data;

namespace VisconSupportAPI.Handlers;

public abstract class Handler<T>
{
    protected readonly ILogger<T> Logger;

    protected readonly DatabaseContext Context;

    protected readonly IConfiguration Configuration;

    protected Handler(ILogger<T> logger, DatabaseContext context, IConfiguration configuration)
    {
        Logger = logger;
        Context = context;
        Configuration = configuration;
    }
}