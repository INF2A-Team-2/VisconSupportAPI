using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VisconSupportAPI.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("rowid")]
    public long Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public long? PhoneNumber { get; set; }
    public string? Unit { get; set; }
}