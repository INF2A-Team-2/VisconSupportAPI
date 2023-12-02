using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public class Log
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public string Description { get; set; }
    public int? IssueId { get; set; }
    public int? UserId { get; set; }
    public int? MachineId { get; set; }
    public int? MessageId { get; set; }
    public int? AttachmentId { get; set; }
    [JsonIgnore] public User Author { get; set; }
    [JsonIgnore] public Issue? Issue { get; set; }
    [JsonIgnore] public User? User { get; set; }
    [JsonIgnore] public Machine? Machine { get; set; }
    [JsonIgnore] public Message? Message { get; set; }
    [JsonIgnore] public Attachment? Attachment { get; set; }
}