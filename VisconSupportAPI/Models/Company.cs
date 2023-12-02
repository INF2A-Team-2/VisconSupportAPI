using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]public List<User> Employees { get; set; }
    [JsonIgnore]public List<Machine> Machines { get; set; }
}