using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public class Machine
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore] public List<Issue> Issues { get; set; }
    [JsonIgnore] public List<Company> Companies { get; set; }
}