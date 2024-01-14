namespace VisconSupportAPI.Models;

public class ForgotPassword
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public User User { get; set; }
}