using System.Text.Json.Serialization;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Models;

public class Report
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime TimeStamp { get; set; }
    public int CompanyId { get; set; }
    public int MachineId { get; set; }
    public bool Public { get; set; }
    [JsonIgnore] public Company Company { get; set; }
    [JsonIgnore] public Machine Machine { get; set; }
}