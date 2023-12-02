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
    public DbSet<Log> Logs { get; set; }

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
        
        modelBuilder.Entity<User>()
            .HasMany<Machine>(x => x.Machines)
            .WithMany();

        modelBuilder.Entity<Log>()
            .HasOne<User>(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();

        modelBuilder.Entity<Log>()
            .HasOne<User>(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Log>()
            .HasOne<Issue>(x => x.Issue)
            .WithMany()
            .HasForeignKey(x => x.IssueId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Log>()
            .HasOne<Machine>(x => x.Machine)
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Log>()
            .HasOne<Message>(x => x.Message)
            .WithMany()
            .HasForeignKey(x => x.MessageId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        modelBuilder.Entity<Log>()
            .HasOne<Attachment>(x => x.Attachment)
            .WithMany()
            .HasForeignKey(x => x.AttachmentId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
    }
}