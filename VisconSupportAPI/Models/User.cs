using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public enum AccountType
{
    User,
    Helpdesk,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    [JsonIgnore] public string PasswordHash { get; set; }
    public AccountType Type { get; set; }
    public string? PhoneNumber { get; set; }
    public int? UnitId { get; set; }
    public int? CompanyId { get; set; }
    [JsonIgnore] public Company? Company { get; set; }
    [JsonIgnore] public List<Issue> Issues { get; set; }
    [JsonIgnore] public List<Message> Messages { get; set; }
}