using System.Text.Json.Serialization;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    [JsonIgnore] public string PasswordHash { get; set; }
    public AccountType Type { get; set; }
    public int? UnitId { get; set; }
    public int? CompanyId { get; set; }
    public string Email { get; set; }
    [JsonIgnore] public Company? Company { get; set; }
    [JsonIgnore] public List<Issue> Issues { get; set; }
    [JsonIgnore] public List<Message> Messages { get; set; }
    [JsonIgnore] public PasswordResetSession? PasswordResetSession { get; set; }
}