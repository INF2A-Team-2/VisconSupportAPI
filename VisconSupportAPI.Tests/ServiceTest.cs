using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;
using VisconSupportAPI.Services;

namespace VisconSupportAPI.Tests;

public class ServiceTest : BaseTest
{
    protected readonly ServicesList Services;

    protected ServiceTest() : base()
    {
        Services = new ServicesList(Context, Config);
    }
}