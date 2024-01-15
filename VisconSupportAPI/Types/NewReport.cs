namespace VisconSupportAPI.Controllers;

public class NewReport
{
    public string Title { get; set; }
    public string Body { get; set; }
    public bool Public { get; set; }
    public int IssueId { get; set; }
    public int MachineId { get; set; }
}