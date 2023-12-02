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
}