using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;


[Collection("test")]
public class CompanyTest : ServiceTest
{
    public CompanyTest() : base() {}


    public Company CreateCompanyTest()
    {
        return Services.Companies.GetById(2) ?? Services.Companies.Create(new NewCompany()
        {
            Name = "TestCompany",
            Latitude = 20,
            Longitude = 30,
            PhoneNumber = "31612345678"
        });
    }
    
    
    
    [Theory]
    [InlineData("test", "test", true)]
    [InlineData(null, null,  false)]
    public void TestCreate(string? name, string? phonenumber, bool valid)
    {
        bool passed = true;

        Company? company = null;

        try
        {
            company = Services.Companies.Create(new NewCompany()
            {
                Name = name,
                PhoneNumber = phonenumber
            });
        }
        catch (Exception)
        {
            passed = false;
        }
        
        Assert.Equal(valid, passed);

        if (company != null)
        {
            Services.Users.Delete(company.Id);
        }
    }
    
    [Theory]
    [InlineData(true, "someothername", false)]
    [InlineData(false, "someothername", true)]
    [InlineData(false, "customer", false)]
    public void TestEdit(bool isUserNull, string username, bool valid)
    {
        Company? company = isUserNull ? null : CreateCompanyTest();

        bool passed = true;

        try
        {
            Services.Companies.Edit(company?.Id ?? -1, new NewCompany()
            {
                Name = username
            });
        }
        catch (Exception)
        {
            passed = false;
        }
        
        Assert.Equal(valid, passed);

        if (company != null)
        {
            Services.Companies.Delete(company.Id);
        }
    }
    
    [Fact]
    public void TestDelete()
    {
        Company company = CreateCompanyTest();
        
        Services.Companies.Delete(company.Id);
        
        Assert.DoesNotContain(company, Services.Companies.GetAll());
    }
}