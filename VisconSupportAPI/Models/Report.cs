using System.Text.Json.Serialization;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Models;

public class Report
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime TimeStamp { get; set; }
    public int MachineId { get; set; }
    [JsonIgnore] public List<Company> Companies { get; set; }
    [JsonIgnore] public Machine Machine { get; set; }
}