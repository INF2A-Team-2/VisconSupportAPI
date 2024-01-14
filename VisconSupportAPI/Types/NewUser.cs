namespace VisconSupportAPI.Types;

public class NewUser
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public AccountType Type { get; set; }
    public int? UnitId { get; set; }
    public int? CompanyId { get; set; }
    public string? Email { get; set; }
}