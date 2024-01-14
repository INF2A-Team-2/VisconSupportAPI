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
        User? user = Context.Users.FirstOrDefault(h => h.Username == "testuser");
        if (user != null)
        {
            Context.Users.Remove(user);
        }

        User? user2 = Context.Users.FirstOrDefault(h => h.Username == "issuetestuser");
        if (user2 != null)
        {
            Context.Users.Remove(user2);
        }

        Context.Machines.Where(h => h.Name == "machine").ToList()
            .ForEach(h => Context.Machines.Remove(h));
    }
}