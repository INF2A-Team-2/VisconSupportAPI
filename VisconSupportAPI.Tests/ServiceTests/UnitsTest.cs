using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisconSupportAPI.Data;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

[Collection("Tests")]
public class UnitsTest : ServiceTest
{
    public UnitsTest() : base() {}

    public Unit CreateUnitModel()
    {
        return Services.Units.GetByName("testunit") ?? Services.Units.Create(new NewUnit()
        {
            Name = "testunit",
            Description = "testdescription"
        });
    }

    [Theory]
    [InlineData(0, false)]
    public void TestGetById(int UnitId, bool exists)
    {
        CreateUnitModel();
        Unit? Unit = Services.Units.GetById(UnitId);

        Assert.Equal(exists, Unit != null);
    }

    [Fact]
    public void TestGetAll()
    {
        CreateUnitModel();
        List<Unit> Units = Services.Units.GetAll();
        Assert.NotEmpty(Units);
    }

    [Theory]
    [InlineData("extraunit", true)]
    public void TestCreate(string name, bool valid)
    {
        bool passed = true;
        CreateUnitModel();
        try
        {
            Services.Units.Create(new NewUnit()
            {
                Name = name,
                Description = "testdescription"
            });
        }
        catch (ArgumentException)
        {
            passed = false;
        }
        Assert.Equal(valid, passed);
    }

    [Theory]
    [InlineData("testunit", true)]
    public void TestEdit(string name, bool valid)
    {
        bool passed = true;
        Unit unit = CreateUnitModel();
        try
        {
            Services.Units.Edit(unit.Id, new NewUnit()
            {
                Name = name,
                Description = "testdescription"
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
        bool passed = true;
        try
        {
            Unit unit = CreateUnitModel();
            Services.Units.Delete(unit.Id);
            Unit Unit2 = Services.Units.GetByName("extraunit");
            Services.Units.Delete(Unit2.Id);
        }
        catch (ArgumentException)
        {
            passed = false;
        }

        Assert.True(passed);
    }
}