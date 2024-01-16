using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;
namespace VisconSupportAPI.Services;

public class UnitService : Service
{
    public UnitService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Unit? GetById(int id) => Context.Units.FirstOrDefault(u => u.Id == id);

    public Unit? GetByName(string name) => Context.Units.FirstOrDefault(u => u.Name == name);

    public List<Unit> GetAll() => Context.Units.ToList();

    public Unit Create(NewUnit data)
    {
        Unit unit = new Unit()
        {
            Name = data.Name,
            Description = data.Description
        };

        Context.Units.Add(unit);
        Context.SaveChanges();

        return unit;
    }

    public Boolean Edit(int id, NewUnit data)
    {
        Unit? unit = GetById(id);

        if (unit == null)
        {
            return false;
        }

        unit.Name = data.Name;
        unit.Description = data.Description;

        Context.SaveChanges();
        return true;
    }

    public Boolean Delete(int id)
    {
        Unit? unit = GetById(id);

        if (unit != null)
        {
            Context.Units.Remove(unit);
            Context.SaveChanges();
            return true;
        }
        return false;
    }
}
