using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Tests;

[Collection("MachineTests")]
public class MachineTests : ServiceTest
{
    public MachineTests() : base() {}

    public Machine CreateTestMachine()
    {
        return Services.Machines.GetByName("testmachine") ?? Services.Machines.Create(new NewMachine()
        {
            Name = "testmachine"
        });
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void TestGetById(int machineId, bool exists)
    {
        Machine? machine = Services.Machines.GetById(machineId);

        Assert.Equal(exists, machine != null);
    }

    [Theory]
    [InlineData("machine", true)]
    [InlineData("nonexistentmachine", false)]
    public void TestGetByName(string name, bool exists)
    {
        Machine? machine = Services.Machines.GetByName(name);

        Assert.Equal(exists, machine != null);
    }

    [Fact]
    public void TestGetAll()
    {
        List<Machine> machines = Services.Machines.GetAll();

        Assert.Single(machines);
    }

    [Fact]
    public void TestGetAllForUser()
    {
        User user = Services.Users.GetByUsername("customer") ?? throw new Exception("User not found");

        List<Machine> machines = Services.Machines.GetAllForUser(user);

        Assert.Single(machines);
    }

    [Theory]
    [InlineData("testmachine", true)]
    [InlineData("machine", false)]
    public void TestCreate(string name, bool valid)
    {
        bool passed = true;

        Machine machine = null;
        try
        {
            machine = Services.Machines.Create(new NewMachine()
            {
                Name = name
            });
        }
        catch (ArgumentException)
        {
            passed = false;
        }

        Assert.Equal(valid, passed);

        if (machine != null)
        {
            Services.Machines.Delete(machine.Id);
        }
    }

    [Theory]
    [InlineData(true, "someothername", false)]
    [InlineData(false, "someothername", true)]
    [InlineData(false, "machine", false)]
    public void TestEdit(bool isMachineNull, string name, bool valid)
    {
        Machine? machine = isMachineNull ? null : CreateTestMachine();

        bool passed = true;

        try
        {
            Services.Machines.Edit(machine?.Id ?? -1, new NewMachine()
            {
                Name = name
            });
        }
        catch (ArgumentException)
        {
            passed = false;
        }

        Assert.Equal(valid, passed);

        if (machine != null)
        {
            Services.Machines.Delete(machine.Id);
        }
    }

    [Fact]
    public void TestDelete()
    {
        Machine machine = CreateTestMachine();

        Services.Machines.Delete(machine.Id);

        Assert.Null(Services.Machines.GetById(machine.Id));
    }
}