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
    public DbSet<FileChunk> FileChunks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // foreign keys for issue
        modelBuilder.Entity<User>()
            .HasMany<Issue>()
            .WithOne()
            .HasForeignKey(h => h.UserId)
            .IsRequired();
        
        modelBuilder.Entity<Machine>()
            .HasMany<Issue>()
            .WithOne()
            .HasForeignKey(h => h.MachineId)
            .IsRequired();
        
        // foreign keys for message
        modelBuilder.Entity<Message>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .IsRequired();
        
        modelBuilder.Entity<Message>()
            .HasOne<Issue>()
            .WithMany()
            .HasForeignKey(h => h.IssueId)
            .IsRequired();
        
        // foreign keys for attachment
        modelBuilder.Entity<Attachment>()
            .HasOne<Issue>()
            .WithMany(h => h.Attachments)
            .HasForeignKey(h => h.IssueId)
            .IsRequired();

        modelBuilder.Entity<Attachment>()
            .HasMany<FileChunk>(h => h.Chunks)
            .WithOne()
            .HasForeignKey(h => h.AttachmentID)
            .IsRequired();
        
        // foreign key for user to machines
        modelBuilder.Entity<User>()
            .HasMany(h => h.Machines)
            .WithMany();
    }
}