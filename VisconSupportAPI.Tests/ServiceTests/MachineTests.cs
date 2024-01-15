using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

[Collection("Tests")]
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
    [InlineData("testmachine", true)]
    [InlineData("nonexistentmachine", false)]
    public void TestGetByName(string name, bool exists)
    {
        CreateTestMachine();
        Machine? machine = Services.Machines.GetByName(name);

        Assert.Equal(exists, machine != null);
    }

    [Fact]
    public void TestGetAll()
    {
        List<Machine> machines = Services.Machines.GetAll();

        Assert.NotEmpty(machines);
    }

    [Fact]
    public void TestGetAllForUser()
    {
        User user = Services.Users.GetByUsername("customer") ?? throw new Exception("User not found");

        List<Machine> machines = Services.Machines.GetAllForUser(user);

        Assert.Single(machines);
    }

    [Theory]
    [InlineData("testmachine", false)]
    [InlineData("machine", true)]
    public void TestCreate(string name, bool valid)
    {
        bool passed = true;
        CreateTestMachine();

        try
        {
            Services.Machines.Create(new NewMachine()
            {
                Name = name
            });
        }
        catch (ArgumentException)
        {
            passed = false;
        }

        Assert.Equal(valid, passed);
    }

    [Theory]
    [InlineData("testmachine", true)]
    public void TestEdit(string name, bool valid)
    {
        Machine machine = CreateTestMachine();

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
    }

    [Fact]
    public void TestDelete()
    {
        Machine machine = CreateTestMachine();

        Services.Machines.Delete(machine.Id);

        Assert.Null(Services.Machines.GetById(machine.Id));
    }
}