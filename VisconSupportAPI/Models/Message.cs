namespace VisconSupportAPI.Models;

public class Message
{
    public long Id { get; set; }
    public string Body { get; set; }
    public DateTime TimeStamp { get; set; }
    // Has a user and issue as foreign key in dbcontext
}