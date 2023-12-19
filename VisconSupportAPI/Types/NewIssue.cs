using VisconSupportAPI.Types;

namespace VisconSupportAPI.Controllers;

public class NewIssue
{
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public string Actual { get; set; }
    public string Expected { get; set; }
    public string Tried { get; set; }
    public string Headline { get; set; }
    public int MachineId { get; set; }
}