using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

public class ServiceTest : BaseTest, IDisposable
{
    protected readonly ServicesList Services;

    protected ServiceTest() : base()
    {
        Services = new ServicesList(Context, Config);
    }

    public void Dispose()
    {
        //Context.Users.Where(u => u.Username.Contains("Test")).
        Context.Machines.Where(h => h.Name == "machine").ToList()
            .ForEach(h => Services.Machines.Delete(h.Id));
        
        Context.Machines.Where(u => u.Name.Contains("test")).ToList()
            .ForEach(u => Services.Users.Delete(u.Id));
    }
}