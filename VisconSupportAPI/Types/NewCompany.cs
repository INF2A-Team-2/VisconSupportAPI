using VisconSupportAPI.Models;

namespace VisconSupportAPI.Types;

public class NewCompany
{
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}