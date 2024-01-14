using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

[Collection("IssueTests")]
public class IssueTests : ServiceTest
{
    public IssueTests() : base()
    {}

    public Machine CreateTestMachine()
    {
        return Services.Machines.GetByName("Illuminator") ?? Services.Machines.Create(new NewMachine()
        {
            Name = "Illuminator"
        });
    }
    
    public User CreateTestUser()
    {
        return Services.Users.GetByUsername("testuser") ?? Services.Users.Create(new NewUser()
        {
            Username = "testuser",
            Password = "test",
        });
    }
    public Issue CreateTestIssue()
    {
        var machineId = CreateTestMachine().Id;
        return Services.Issues.GetById(1) ?? Services.Issues.Create(new NewIssue()
        {
            Actual = "My machine is broken",
            Expected = "My machine is working",
            Tried = "I tried to fix it",
            PhoneNumber = "0612345678",
            Headline = "Machine broken",
            MachineId = machineId,
            Priority = Priority.High,
            Status = Status.Open,
        }, CreateTestUser());
    }
    
    [Fact]
    public void TestGetById()
    {
        var id = CreateTestIssue().Id;
        Issue? issue = Services.Issues.GetById(id);

        Assert.NotNull(issue);
    }
    
    [Fact]
    public void TestGetAll()
    {
        CreateTestIssue();
        List<Issue> issues = Services.Issues.GetAll();
        
        Assert.NotEmpty(issues);
    }
    
    [Fact]
    public void TestCreate()
    {
        bool passed = true;

        Issue? issue = null;

        try
        {
            issue = CreateTestIssue();
        }
        catch (Exception e)
        {
            passed = false;
        }

        Assert.True(passed);
        Assert.NotNull(issue);
    }
    
    [Fact]
    public void TestEdit()
    {
        bool passed;

        Issue issue = CreateTestIssue();
        issue.Priority = Priority.Low;
        
        try
        {
            issue = CreateTestIssue();
            passed = Services.Issues.Edit(issue.Id, issue, CreateTestUser());
        }
        catch (Exception e)
        {
            passed = false;
        }

        Assert.True(passed);
        Assert.NotNull(issue);
        
    }
    
    [Fact]
    public void TestDelete()
    {
        bool passed = true;

        Issue issue = CreateTestIssue();
        
        try
        {
            Services.Issues.Delete(issue.Id);
        }
        catch (Exception e)
        {
            passed = false;
        }

        Assert.True(passed);
    }
}