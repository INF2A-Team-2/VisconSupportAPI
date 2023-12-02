using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

[Collection("UserTests")]
public class UserTests : ServiceTest
{
    public UserTests() : base()
    {
        
    }

    public User CreateTestUser()
    {
        return Services.Users.GetByUsername("testuser") ?? Services.Users.Create(new NewUser()
        {
            Username = "testuser",
            Password = "test",
            CompanyId = 1
        });
    }
    
    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void TestGetById(int userId, bool exists)
    {
        User? user = Services.Users.GetById(userId);

        Assert.Equal(exists, user != null);
    }
    
    [Theory]
    [InlineData("customer", true)]
    [InlineData("nonexistentuser", false)]
    public void TestGetByUsername(string username, bool exists)
    {
        User? user = Services.Users.GetByUsername(username);
        
        Assert.Equal(exists, user != null);
    }
    
    [Fact]
    public void TestGetAll()
    {
        List<User> users = Services.Users.GetAll();
        
        Assert.Equal(3, users.Count);
    }
    
    [Theory]
    [InlineData("test", "test", true)]
    [InlineData(null, null, false)]
    public void TestCreate(string? username, string? password, bool valid)
    {
        bool passed = true;

        User? user = null;

        try
        {
            user = Services.Users.Create(new NewUser()
            {
                Username = username,
                Password = password
            });
        }
        catch (Exception)
        {
            passed = false;
        }
        
        Assert.Equal(valid, passed);

        if (user != null)
        {
            Services.Users.Delete(user.Id);
        }
    }
    
    [Theory]
    [InlineData(true, "someothername", false)]
    [InlineData(false, "someothername", true)]
    [InlineData(false, "customer", false)]
    public void TestEdit(bool isUserNull, string username, bool valid)
    {
        User? user = isUserNull ? null : CreateTestUser();

        bool passed = true;

        try
        {
            Services.Users.Edit(user?.Id ?? -1, new NewUser()
            {
                Username = username
            });
        }
        catch (Exception)
        {
            passed = false;
        }
        
        Assert.Equal(valid, passed);

        if (user != null)
        {
            Services.Users.Delete(user.Id);
        }
    }
    
    [Theory]
    [InlineData(true, 0, false)]
    [InlineData(false, 4, true)]
    [InlineData(false, 0, true)]
    public void TestAddMachine(bool isUserNull, int machineId, bool valid)
    {
        User? user = isUserNull ? null : CreateTestUser();

        Machine? machine = machineId > 0 ? Services.Machines.GetById(machineId) : new Machine(){ Name = "testmachine" };

        if (machine == null)
        {
            Assert.Fail($"Machine with ID {machineId} doesn't exist");
        }

        bool passed = true;

        try
        {
            Services.Users.AddMachine(user?.Id ?? -1, machine);
        }
        catch (Exception)
        {
            passed = false;
        }
        
        Assert.Equal(valid, passed);
        
        if (user != null)
        {
            Services.Users.Delete(user.Id);
        }

        if (machineId <= 0)
        {
            Services.Machines.Delete(machine.Id);
        }
    }
    
    [Theory]
    [InlineData(true, false)]
    public void TestEditMachines(bool isUserNull, bool valid)
    {
        User? user = isUserNull ? null : CreateTestUser();

        List<Machine> machines = new List<Machine>()
        {
            new Machine() { Name = "machine1" },
            new Machine() { Name = "machine2" },
        };

        bool passed = true;

        try
        {
            Services.Users.EditMachines(user?.Id ?? -1, machines);
        }
        catch (Exception)
        {
            passed = false;
        }
        
        Assert.Equal(valid, passed);
        
        if (user != null)
        {
            Services.Users.Delete(user.Id);
        }

        machines.ForEach(m => Services.Machines.Delete(m.Id));
    }
    
    [Fact]
    public void TestDelete()
    {
        User user = CreateTestUser();
        
        Services.Users.Delete(user.Id);
        
        Assert.DoesNotContain(user, Services.Users.GetAll());
    }
}