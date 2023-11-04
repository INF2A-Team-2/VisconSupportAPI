using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VisconSupportAPI.Data;
using Bogus;
using VisconSupportAPI.Models;
using VisconSupportAPI.Controllers;

namespace VisconSupportAPI.Seeder;

public static class Program
{
    private static readonly IConfiguration Config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    
    private static readonly DatabaseContext Context = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
        .UseNpgsql(Config.GetConnectionString("Database"))
        .Options);
    
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Missing required arguments: table, amount");
            return;
        }
        
        if (args.Length == 1)
        {
            args = new[] { args[0], "10" };
        }
        
        Action<int> seedFunc = args[0].ToLower() switch
        {
            "clear" => Clear,
            "all" => SeedAll,
            "issues" => SeedIssues,
            "machines" => SeedMachines,
            "users" => SeedUsers,
            _ => throw new ArgumentOutOfRangeException(nameof(args), args[0].ToLower(), "Invalid table to seed")
        };

        if (!int.TryParse(args[1], out int amount))
        {
            Console.WriteLine("Amount must be an integer");
            return;
        }
        
        if (amount <= 0)
        {
            Console.WriteLine("Amount must be a positive integer");
            return;
        }

        seedFunc(amount);
    }

    public static void Clear(int amount)
    {
        Context.Users.Where(u => u.Unit != "Development").ExecuteDelete();
        Context.Machines.ExecuteDelete();
        Context.Issues.ExecuteDelete();
        Context.Messages.ExecuteDelete();
        
        Console.WriteLine("Cleared all tables");
    }

    public static void SeedAll(int amount)
    {
        SeedMachines(amount);
        SeedUsers(amount);
        SeedIssues(amount);
    }

    public static void SeedIssues(int amount)
    {
        Console.WriteLine("Seeding issues...");

        List<User> users = Context.Users.Include(user => user.Machines).ToList();

        long userId = 0;
        Faker<Issue> issueFaker = new Faker<Issue>()
            .RuleFor(i => i.Headline, f => f.Lorem.Sentence())
            .RuleFor(i => i.Actual, f => f.Lorem.Sentences())
            .RuleFor(i => i.Expected, f => f.Lorem.Sentences())
            .RuleFor(i => i.Tried, f => f.Lorem.Sentences())
            .RuleFor(i => i.TimeStamp, f => f.Date.Past().ToUniversalTime())
            .RuleFor(i => i.UserId, f =>
            {
                userId = f.PickRandom(users.Where(u => u.Type == AccountType.User).ToList()).Id;
                return userId;
            })
            .RuleFor(i => i.MachineId, f => f.PickRandom(users.First(u => u.Id == userId).Machines).Id);

        List<Issue> issues = issueFaker.Generate(amount);
        Context.Issues.AddRange(issues);
        Context.SaveChanges();
        
        issues.ForEach(i =>
        {
            Faker<Message> messageFaker = new Faker<Message>()
                .RuleFor(m => m.Body, f => f.Lorem.Sentences())
                .RuleFor(m => m.TimeStamp, f => f.Date.Soon(5, i.TimeStamp).ToUniversalTime())
                .RuleFor(m => m.UserId, f => f.PickRandom(users.ToList()).Id)
                .RuleFor(m => m.IssueId, i.Id);

            List<Message> messages = messageFaker.Generate(10);
            Context.Messages.AddRange(messages);
            Context.SaveChanges();
            
            Console.WriteLine($"ID: {i.Id}; TimeStamp: {i.TimeStamp}; Headline: {i.Headline}; Actual: {i.Actual}; Expected: {i.Expected}; Tried: {i.Tried}; UserID: {i.UserId}; MachineID: {i.MachineId}");
            
            messages.ForEach(m => Console.WriteLine($"ID: {m.Id}; TimeStamp: {m.TimeStamp}; Body: {m.Body}; UserID: {m.UserId}; IssueID: {m.IssueId}"));
        });
    }
    
    public static void SeedMachines(int amount)
    {
        Console.WriteLine("Seeding Machines...");

        Faker<Machine> machineFaker = new Faker<Machine>()
            .RuleFor(m => m.Name, f => $"{f.Hacker.Adjective()} {f.Hacker.Noun()}");

        List<Machine> machines = machineFaker.Generate(amount);
        Context.Machines.AddRange(machines);
        Context.SaveChanges();
        
        machines.ForEach(m => Console.WriteLine($"ID: {m.Id}; Username: {m.Name}"));
    }
    
    public static void SeedUsers(int amount)
    {
        Console.WriteLine("Seeding Users...");

        string[] units = new[]
        {
            "Flowers & Plants",
            "Fruit & Vegetables",
            "Poultry",
            "Production Logistics",
            "Insects",
            "Warehousing & Fulfillment"
        };
        
        List<string> usernames = new List<string>();
        List<string> passwords = new List<string>();

        AccountType accType = AccountType.User;
        Faker<User> userFaker = new Faker<User>()
            .RuleFor(u => u.Username, f =>
            {
                while (true)
                {
                    string name = f.Name.FirstName().ToLower();

                    if (usernames.Contains(name))
                    {
                        continue;
                    }
                    
                    usernames.Add(name);
                    return name;
                }
            })
            .RuleFor(u => u.Type, f =>
            {
                accType = f.PickRandom(new[] { AccountType.User, AccountType.Helpdesk });
                return accType;
            })
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.PasswordHash, f =>
            {
                string pass = f.Random.Word();
                passwords.Add(pass);
                return AuthController.HashPassword(pass);
            })
            .RuleFor(u => u.Unit, f => f.PickRandom(units))
            .RuleFor(u => u.Machines, f => accType == AccountType.User ? f.Random.ListItems(Context.Machines.ToList(), f.Random.Number(1, 5)) : new List<Machine>());

        List<User> users = userFaker.Generate(amount);
        Context.Users.AddRange(users);
        Context.SaveChanges();

        if (File.Exists("users"))
        {
            File.Delete("users");
        }

        int i = 0;
        File.WriteAllLines("users", users.Select(u => $"{u.Username} - {passwords[i++]}"));

        int j = 0;
        users.ForEach(u => Console.WriteLine($"ID: {u.Id}; Username: {u.Username}; Password: {passwords[j++]}, Type: {u.Type}; PhoneNumber: {u.PhoneNumber}, Unit: {u.Unit}; Machines: [{string.Join(", ", u.Machines.Select(m => m.Name))}]"));
    }
}