using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Services;

public class AuthService : Service
{
    public AuthService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }
    
    public string GenerateJsonWebToken(User user)
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

    public User? AuthenticateUser(UserCredentials credentials)
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