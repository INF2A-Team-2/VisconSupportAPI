using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
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

    public ActionResult ForgotPassword(string email)
    {
        var user = Services.Users.GetAll().FirstOrDefault(u => u.Email == email);
        if(user == null)
        {
            return new NotFoundResult();
        }

        Services.ForgotPassword.GetToken(user);
        return new OkResult();
    }
    
    public ActionResult ResetPassword(string token, string password)
    {
        try
        {
            Services.ForgotPassword.ResetPassword(token, password);
            return new OkResult();
        }
        catch (ArgumentException e)
        {
            return new BadRequestObjectResult(new {error = e.Message});
        }
    }
}