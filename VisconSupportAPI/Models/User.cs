using System.ComponentModel.DataAnnotations.Schema;

namespace VisconSupportAPI.Models;

public enum AccountType
{
    User,
    Helpdesk,
    Admin
}

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("rowid")]
    public long Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public AccountType? Type { get; set; }
    public long? PhoneNumber { get; set; }
    public string? Unit { get; set; }
}