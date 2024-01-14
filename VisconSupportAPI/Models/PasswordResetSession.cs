using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace VisconSupportAPI.Models;


[Index(nameof(Id), nameof(Token), IsUnique = true)]
public class PasswordResetSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    [JsonIgnore] public User User { get; set; }
}