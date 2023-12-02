using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Services;

public class UserService : Service
{
    public UserService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public User? GetById(int id) => Context.Users.FirstOrDefault(u => u.Id == id);

    public User? GetByUsername(string username) => Context.Users.FirstOrDefault(u => u.Username == username);

    public List<User> GetAll() => Context.Users.ToList();

    public User Create(NewUser data)
    {
        if (data.Username == null || data.Password == null)
        {
            throw new ArgumentNullException(nameof(data), "Username or password cannot be null");
        }
        
        User user = new User()
        {
            Username = data.Username,
            PasswordHash = AuthService.HashPassword(data.Password),
            Type = data.Type,
            PhoneNumber = data.PhoneNumber,
            Unit = data.Unit,
            CompanyId = data.CompanyId
        };
        

        Context.Users.Add(user);

        Context.SaveChanges();

        return user;
    }

    public void Edit(int id, NewUser data)
    {
        User? user = GetById(id);

        if (user == null)
        {
            throw new ArgumentException("User with ID {id} not found", nameof(id));
        }

        if (data.Username != null && 
            Context.Users.Select(u => u.Username)
                .Where(u => u != user.Username)
                .Contains(data.Username))
        {
            throw new ArgumentException($"A user with username \"{data.Username}\" already exists", nameof(data));
        }

        user.Username = data.Username ?? user.Username;

        if (data.Password != null)
        {
            user.PasswordHash = AuthService.HashPassword(data.Password);
        }

        user.Type = data.Type;
        user.PhoneNumber = data.PhoneNumber;
        user.Unit = data.Unit;
        
        if (user.Type != AccountType.User)
        {
            Context.Entry(user).Collection(u => u.Company.Machines).Load();
            user.Company.Machines.Clear();
        }

        Context.SaveChanges();
    }

    public void AddMachine(int id, Machine machine)
    {
        User? user = GetById(id);

        if (user == null)
        {
            throw new ArgumentException("User with ID {id} not found", nameof(id));
        }

        if (!Services.Machines.GetAll().Contains(machine))
        {
            Context.Machines.Add(machine);

            Context.SaveChanges();
        }

        Services.LoadCollection(user, u => u.Company.Machines);

        if (!user.Company.Machines.Contains(machine))
        {
            user.Company.Machines.Add(machine);
        }
        
        Context.SaveChanges();
    }

    public void EditMachines(int id, List<Machine> machines)
    {
        User? user = GetById(id);

        if (user == null)
        {
            throw new ArgumentException("User with ID {id} not found", nameof(id));
        }

        Services.LoadCollection(user, u => u.Company.Machines);
        
        user.Company.Machines.Clear();
        
        machines.ForEach(m => AddMachine(user.Id, m));
    }

    public void Delete(int id)
    {
        User? user = GetById(id);

        if (user != null)
        {
            Context.Users.Remove(user);
            Context.SaveChanges();
        }
    }
}