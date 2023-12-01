using Microsoft.EntityFrameworkCore;

using VisconSupportAPI.Models;
using VisconSupportAPI.Services;

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
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Issue>()
            .HasOne<User>(x => x.User)
            .WithMany(x => x.Issues)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Issue>()
            .HasOne<Machine>(x => x.Machine)
            .WithMany(x => x.Issues)
            .HasForeignKey(x => x.MachineId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Message>()
            .HasOne<User>(x => x.User)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Issue>()
            .HasMany<Message>(x => x.Messages)
            .WithOne(x => x.Issue)
            .HasForeignKey(x => x.IssueId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Issue>()
            .HasMany<Attachment>(x => x.Attachments)
            .WithOne(x => x.issue)
            .HasForeignKey(x => x.IssueId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        // foreign key for user to machines
        modelBuilder.Entity<User>()
            .HasMany<Machine>(h => h.Machines)
            .WithMany();
    }
}