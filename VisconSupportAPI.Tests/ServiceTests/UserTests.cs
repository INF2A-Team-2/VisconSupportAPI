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
        CreateTestUser();
    }

    public User CreateTestUser()
    {
        return Services.Users.GetByUsername("testuser") ?? Services.Users.Create(new NewUser()
        {
            Username = "testuser",
            Password = "test",
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
        
        Assert.InRange(users.Count, 3, 5);
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
    
    [Fact]
    public void TestDelete()
    {
        User user = CreateTestUser();
        
        Services.Users.Delete(user.Id);
        
        Assert.DoesNotContain(user, Services.Users.GetAll());
    }
}