namespace VisconSupportAPI.Models;

public class Message
{
    public long Id { get; set; }
    public string Body { get; set; }
    public DateTime TimeStamp { get; set; }
    public long UserId { get; set; }
    public long IssueId { get; set; }
}