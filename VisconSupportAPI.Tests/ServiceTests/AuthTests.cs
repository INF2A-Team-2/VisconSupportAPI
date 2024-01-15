using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Services;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

[Collection("Tests")]
public class AuthTests : ServiceTest
{
    public AuthTests() : base()
    {
        
    }

    [Fact]
    public void TestJsonWebToken()
    {
        User? user = Services.Users.GetById(1);
        
        Assert.NotNull(user);
        
        string jwt = Services.Auth.GenerateJsonWebToken(user);

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = handler.ReadJwtToken(jwt);
        
        string? name = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        
        Assert.NotNull(name);
        Assert.Equal(user.Username, name);
    }

    [Theory]
    [InlineData("customer", "    ", true)]
    [InlineData("employee", "    ", true)]
    [InlineData("admin", "apple", false)]
    public void TestAuthenticateUser(string username, string password, bool valid)
    {
        User? user = Services.Auth.AuthenticateUser(new UserCredentials()
        {
            Username = username,
            Password = password
        });
        
        Assert.Equal(valid, user != null);
    }

    [Theory]
    [InlineData("    ", "Gg9WTdxgOUV7L7JrPWoxbBXrogqIZEmEfDIQw1ghppM=")]
    public void TestHashPassword(string password, string result)
    {
        string hashedPassword = AuthService.HashPassword(password);
        
        Assert.Equal(result, hashedPassword);
    }
}