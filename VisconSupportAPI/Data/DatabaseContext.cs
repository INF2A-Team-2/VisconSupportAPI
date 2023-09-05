using Microsoft.EntityFrameworkCore;

using VisconSupportAPI.Models;

namespace VisconSupportAPI.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    
    public DbSet<Test> Test { get; set; }
}