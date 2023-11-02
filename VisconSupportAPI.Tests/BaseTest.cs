using Microsoft.Extensions.Configuration;

namespace VisconSupportAPI.Tests;

public class BaseTest
{
    protected IConfiguration Config;
    protected HttpClient HttpClient;
    
    public BaseTest()
    {
        this.Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testconfig.json")
            .Build();
        
        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
        this.HttpClient = factory.CreateDefaultClient();
    }
}