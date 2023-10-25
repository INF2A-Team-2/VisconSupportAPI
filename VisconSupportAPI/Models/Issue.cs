public class Issue
{
    public long Id {get; set;}
    public string Headline {get; set;}
    public string Actual {get; set;}
    public string Expected {get; set;}
    public string Tried {get; set;}
    public DateTime TimeStamp { get; set; }
    public long UserId { get; set; }
    public long MachineId { get; set; }
}