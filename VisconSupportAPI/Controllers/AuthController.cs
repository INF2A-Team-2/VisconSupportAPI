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
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private readonly DatabaseContext _context;

    private readonly IConfiguration _configuration;

    public AuthController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost]
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

    // [HttpGet]
    // [Authorize]
    // public ActionResult<string> Test()
    // {
    //     return "Test";
    // }

    [HttpGet]
    [Authorize]
    public ActionResult<User> GetUser()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return NotFound();
        }
        
        User? user = _context.Users.FirstOrDefault(h => h.Username == userId);
        
        if (user == null)
        {
            return NotFound();
        }

        _context.Entry(user).State = EntityState.Detached;

        user.PasswordHash = "";
        
        return Ok(user);    
    }

    [HttpGet]
    [Route("/api/login/hash-password")]
    public ActionResult<string> GetHashPassword([FromQuery ]string password)
    {
        string hashedPassword = HashPassword(password);

        return Ok(hashedPassword);
    }

    private string GenerateJSONWebToken(User user)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username)
        };
        
        JwtSecurityToken token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"], 
            claims,
            expires: DateTime.Now.AddHours(2), 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private User? AuthenticateUser(UserCredentials credentials)
    {
        User? user = _context.Users.SingleOrDefault(u => u.Username == credentials.Username);

        if (user == null || user.PasswordHash != HashPassword(credentials.Password))
        {
            return null;
        }

        return user;
    }
    
    private string HashPassword(string input)
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