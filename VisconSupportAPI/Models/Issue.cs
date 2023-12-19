using System.Text.Json.Serialization;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Models;

public class Issue
{
    public int Id {get; set;}
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public string Headline {get; set;}
    public string Actual {get; set;}
    public string Expected {get; set;}
    public string Tried {get; set;}
    public DateTime TimeStamp { get; set; }
    public int UserId { get; set; }
    [JsonIgnore] public User User { get; set; }
    public int MachineId { get; set; }
    [JsonIgnore] public Machine Machine { get; set; }
    [JsonIgnore] public List<Attachment> Attachments { get; set; }
    [JsonIgnore] public List<Message> Messages { get; set; }
}