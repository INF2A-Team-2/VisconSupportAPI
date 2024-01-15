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

    public User? GetByEmail(string email) => Context.Users.FirstOrDefault(u => u.Email == email);

    public List<User> GetAll() => Context.Users.ToList();

    public User Create(NewUser data)
    {
        if (data.Username == null || data.Password == null || data.Email == null)
        {
            throw new ArgumentNullException(nameof(data), "Username, password or email cannot be null");
        }
        
        User user = new User()
        {
            Username = data.Username,
            PasswordHash = AuthService.HashPassword(data.Password),
            Type = data.Type,
            UnitId = data.UnitId,
            CompanyId = data.CompanyId,
            Email = data.Email
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

        if (data.Type != null)
        {
            user.Type = data.Type;
        }

        if (user.Type == AccountType.Admin)
        {
            if (data.UnitId != null)
            {
                user.UnitId = data.UnitId;
            }

            if (data.CompanyId != null)
            {
                user.CompanyId = data.CompanyId;
            }
        }
        if (data.Email != null)
        {
            user.Email = data.Email;
        }

        user.Type = data.Type;
        user.UnitId = data.UnitId;
        user.CompanyId = data.CompanyId;

        Context.SaveChanges();
    }
    
    public Machine AddMachine(int id, Machine machine)
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

        return machine;
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