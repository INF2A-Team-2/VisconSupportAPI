using VisconSupportAPI.Types;

namespace VisconSupportAPI.Controllers;

public class NewReport
{
    public string Title { get; set; }
    public string Body { get; set; }
    public List<int> CompanyIds { get; set; }
    public int MachineId { get; set; }
}