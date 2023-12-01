using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public class Attachment
{
    public int Id {get; set;}
    public string? Name { get; set; }
    public string MimeType { get; set; }
    public int IssueId {get; set;}
    [JsonIgnore] public Issue issue { get; set; }
}