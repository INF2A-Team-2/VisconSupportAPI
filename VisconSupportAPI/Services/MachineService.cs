using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class MachineService : Service
{
    public MachineService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Machine? GetById(int id) => Context.Machines.FirstOrDefault(m => m.Id == id);
    
    public Machine? GetByName(string name) => Context.Machines.FirstOrDefault(m =>  m.Name == name);

    public List<Machine> GetAll() => Context.Machines.ToList();

    public List<Machine> GetAllForUser(User user)
    {
        return user.Company.Machines;
    }

    public Machine Create(NewMachine data)
    {
        if (GetByName(data.Name) != null)
        {
            throw new ArgumentException($"A machine with name \"{data.Name}\" already exists", nameof(data.Name));
        }

        Machine machine = new Machine()
        {
            Name = data.Name
        };
        
        Context.Machines.Add(machine);

        Context.SaveChanges();

        return machine;
    }

    public void Edit(int id, NewMachine data)
    {
        Machine? machine = GetById(id);

        if (machine == null)
        {
            throw new ArgumentException($"Machine with ID {id} not found", nameof(id));
        }

        machine.Name = data.Name;

        Context.SaveChanges();
    }

    public void Delete(int id)
    {
        Machine? machine = GetById(id);

        if (machine != null)
        {
            Context.Machines.Remove(machine);
            Context.SaveChanges();
        }
    }
}