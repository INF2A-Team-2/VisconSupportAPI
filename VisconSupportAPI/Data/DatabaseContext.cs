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
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Unit> Units { get; set; } 
    public DbSet<Report> Reports { get; set; }
    public DbSet<PasswordResetSession> PasswordResetSessions { get; set; }

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
            .HasPrincipalKey(x => x.Id);
        
        modelBuilder.Entity<Company>()
            .HasMany<User>(x => x.Employees)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId)
            .HasPrincipalKey(x => x.Id);

        modelBuilder.Entity<Company>()
            .HasMany<Machine>(x => x.Machines)
            .WithMany(x => x.Companies);

        modelBuilder.Entity<Unit>()
            .Property(u => u.Description)
            .HasMaxLength(512);

        modelBuilder.Entity<Report>()
            .HasMany<Company>(x => x.Companies)
            .WithMany(x => x.Reports);
        
        modelBuilder.Entity<Report>()
            .HasOne<Machine>(x => x.Machine)
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();

        modelBuilder.Entity<PasswordResetSession>()
            .HasOne<User>(x => x.User)
            .WithOne(x => x.PasswordResetSession);
    }
}