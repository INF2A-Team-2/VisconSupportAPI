using Microsoft.EntityFrameworkCore;

using VisconSupportAPI.Models;

namespace VisconSupportAPI.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // foreign keys for issue
        modelBuilder.Entity<User>()
            .HasMany<Issue>()
            .WithOne();
        modelBuilder.Entity<Machine>()
            .HasMany<Issue>()
            .WithOne();
        
        // foreign keys for message
        modelBuilder.Entity<Message>()
            .HasOne<User>()
            .WithMany();
        modelBuilder.Entity<Message>()
            .HasOne<Issue>()
            .WithMany();
        
        // foreign key for user to machines
        modelBuilder.Entity<User>()
            .HasMany(h => h.Machines)
            .WithMany();
    }
}