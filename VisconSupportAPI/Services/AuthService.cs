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

    public PasswordResetSession? GetSessionByToken(string token) =>
        Context.PasswordResetSessions.FirstOrDefault(x => x.Token == token);

    public PasswordResetSession CreatePasswordResetSession(User user)
    {
        PasswordResetSession? existingSession = Context.PasswordResetSessions.FirstOrDefault(x => x.UserId == user.Id);

        if (existingSession != null)
        {
            Context.PasswordResetSessions.Remove(existingSession);
        }
        
        PasswordResetSession session = new PasswordResetSession()
        {
            UserId = user.Id,
            Token = GenerateRandomAsciiString(128),
            CreatedAt = DateTime.UtcNow
        };

        Context.PasswordResetSessions.Add(session);
        Context.SaveChanges();

        return session;
    }
    
    public static string GenerateRandomAsciiString(int length)
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] buffer = new byte[length];
            rng.GetBytes(buffer);

            StringBuilder result = new StringBuilder(length);

            foreach (byte b in buffer)
            {
                char urlChar = allowedChars[b % allowedChars.Length];
                result.Append(urlChar);
            }

            return result.ToString();
        }
    }
    
    public static string HashPassword(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}