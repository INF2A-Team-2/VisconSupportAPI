using System.Text.Json.Serialization;

namespace VisconSupportAPI.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [JsonIgnore]public List<User> Employees { get; set; }
    [JsonIgnore]public List<Machine> Machines { get; set; }
}