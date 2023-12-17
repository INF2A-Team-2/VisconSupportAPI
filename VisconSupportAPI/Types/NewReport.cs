using VisconSupportAPI.Types;

namespace VisconSupportAPI.Controllers;

public class NewReport
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public int MachineId { get; set; }
}