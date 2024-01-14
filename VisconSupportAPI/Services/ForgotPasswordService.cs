using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Services;

public class ForgotPasswordService : Service
{
    public ForgotPasswordService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }
    
    public void GetToken(User user)
    {
        var token = Guid.NewGuid().ToString();
        Context.ForgotPasswords.Add(new ForgotPassword()
        {
            Token = token,
            UserId = user.Id
        });
        Services.Mail.Send(user.Email, "Forgot password", $"You forgot your password, click this link to reset: LINK_NAAR_WEBSITE{token}");
    }
    
    public void ResetPassword(string token, string password)
    {
        var forgotPassword = Context.ForgotPasswords.FirstOrDefault(fp => fp.Token == token);
        if(forgotPassword == null)
        {
            throw new ArgumentException("Token not found", nameof(token));
        }
        var user = Context.Users.FirstOrDefault(u => u.Id == forgotPassword.UserId);
        if(user == null)
        {
            throw new ArgumentException("User not found", nameof(user));
        }
        user.PasswordHash = AuthService.HashPassword(password);
        Context.ForgotPasswords.Remove(forgotPassword);
        Context.SaveChanges();
    }
}