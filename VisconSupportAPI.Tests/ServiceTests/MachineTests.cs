using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Tests;

[Collection("MachineTests")]
public class MachineTests : ServiceTest 
{
    public MachineTests() : base() {}

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void TestGetById(int machineId, bool exists)
    {
        Machine? machine = Services.Machines.GetById(machineId);

        Assert.Equal(exists, machine != null);
    }

    [Fact]
    public void TestGetAll()
    {
        List<Machine> machines = Services.Machines.GetAll();

        Assert.Single(machines);
    }
}