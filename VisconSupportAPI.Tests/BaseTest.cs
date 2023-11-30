using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Tests;

public class BaseTest
{
    protected readonly IConfiguration Config;

    protected readonly DatabaseContext Context;

    protected readonly ILogger Logger;

    protected BaseTest()
    {
        Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseNpgsql(Config.GetConnectionString("Database"));

        Context = new DatabaseContext(optionsBuilder.Options);

        LoggerFactory loggerFactory = new LoggerFactory();

        Logger = loggerFactory.CreateLogger<BaseTest>();
    }
}