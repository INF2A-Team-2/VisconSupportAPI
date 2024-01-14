using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class AuthHandler : Handler
{
    public AuthHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }
    
    public ActionResult Login(UserCredentials credentials)
    {
        User? user = Services.Auth.AuthenticateUser(credentials);

        if (user == null)
        {
            return new UnauthorizedResult();
        }

        string token = Services.Auth.GenerateJsonWebToken(user);
        
        return new OkObjectResult(new { token });
    }
    
    public ActionResult<User> GetUser(User? user)
    {
        if (user == null)
        {
            return new NotFoundResult();
        }
        
        return new OkObjectResult(user);    
    }

    public ActionResult CreatePasswordResetSession(string email, StringValues origin)
    {
        User? user = Services.Users.GetByEmail(email);
        
        if (user == null)
        {
            return new NotFoundResult();
        }

        PasswordResetSession session = Services.Auth.CreatePasswordResetSession(user);

        string baseUrl = "https://project-c.zoutigewolf.dev";

        if (origin.Count > 0)
        {
            string? url = origin[0];

            if (url != null)
            {
                baseUrl = url;
            }
        }
        
        Services.Mail.Send(
            user.Email,
            "Reset password",
            $"""
               Click the following link to reset your password. This link expires in 24 hours.
               Do not share this link with anyone.
               
               {baseUrl}/forgot-password?token={session.Token}
            """);
        
        return new OkResult();
    }

    public ActionResult ChangePassword(string token, string password)
    {
        PasswordResetSession? session = Services.Auth.GetSessionByToken(token);

        if (session == null)
        {
            return new NotFoundResult();
        }

        TimeSpan diff = DateTime.UtcNow - session.CreatedAt;

        if (diff.TotalDays > 1)
        {
            return new NotFoundResult();
        }
        
        Services.Users.Edit(session.UserId, new NewUser()
        {
            Password = password
        });

        return new OkResult();
    }
}