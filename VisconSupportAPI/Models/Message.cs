using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public class Message
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime TimeStamp { get; set; }
    public int UserId { get; set; }
    [JsonIgnore] public User User { get; set; }
    public int IssueId { get; set; }
    [JsonIgnore] public Issue Issue { get; set; }
}