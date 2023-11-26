using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/login")]
public class AuthController : BaseController
{
    public AuthController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult Login(UserCredentials credentials)
    {
        User? user = AuthenticateUser(credentials);

        if (user == null)
        {
            return Unauthorized();
        }

        string token = GenerateJSONWebToken(user);
        
        return Ok(new { token = token });
    }
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetUser()
    {
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(new
        {
            ID = user.Id,
            Username = user.Username,
            Unit = user.Unit,
            PhoneNumber = user.PhoneNumber,
            Type = user.Type
        });    
    }
    
    private string GenerateJSONWebToken(User user)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username)
        };
        
        JwtSecurityToken token = new JwtSecurityToken(
            Configuration["Jwt:Issuer"],
            Configuration["Jwt:Issuer"], 
            claims,
            expires: DateTime.Now.AddHours(2), 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private User? AuthenticateUser(UserCredentials credentials)
    {
        User? user = Context.Users.SingleOrDefault(u => u.Username == credentials.Username);

        if (user == null || user.PasswordHash != HashPassword(credentials.Password))
        {
            return null;
        }

        return user;
    }
    
    public static string HashPassword(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

public class UserCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}