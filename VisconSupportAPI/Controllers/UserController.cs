using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : BaseController
{
    public UserController(ILogger<AuthController> logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    [HttpGet]
    [Authorize]
    public ActionResult<List<User>> GetUsers()
    {
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        List<User> users = Context.Users.ToList();

        return Ok(users);
    }
    
    [HttpGet]
    [Authorize]
    [Route("/api/users/{userId:int}")]
    public ActionResult<User> GetUser(int userId)
    {
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        User? requestedUser = Context.Users.FirstOrDefault(u => u.Id == userId);

        return requestedUser != null ? requestedUser : NotFound();
    }

    [HttpPost]
    [Authorize]
    [Route("/api/users")]
    public ActionResult<User> PostUser(UserCreationData data)
    {
        User? user = GetUserFromClaims();

        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        if (data.Username == null || data.Password == null)
        {
            return BadRequest();
        }

        if (Context.Users.Select(u => u.Username).Contains(data.Username))
        {
            return Conflict();
        }

        User createdUser = new User()
        {
            Username = data.Username,
            PasswordHash = AuthController.HashPassword(data.Password),
            Type = data.Type
        };

        Context.Users.Add(createdUser);
        Context.SaveChanges();

        return Created(
            Url.Action("GetUser", "User", new { userId=createdUser.Id}, Request.Scheme) ?? "",
            createdUser);
    }

    [HttpPut]
    [Authorize]
    [Route("/api/users/{userId:int}")]
    public ActionResult PutUser(int userId, UserCreationData data)
    {
        User? user = GetUserFromClaims();

        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);

        if (selectedUser == null)
        {
            return NotFound();
        }

        if (data.Username != null && Context.Users.Select(u => u.Username).Contains(data.Username))
        {
            return Conflict();
        }

        selectedUser.Username = data.Username ?? selectedUser.Username;
        
        selectedUser.PasswordHash = data.Password != null
            ? AuthController.HashPassword(data.Password)
            : selectedUser.PasswordHash;
        
        selectedUser.Type = data.Type;
        selectedUser.PhoneNumber = data.PhoneNumber;
        selectedUser.Unit = data.Unit;

        Context.SaveChanges();

        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    [Route("/api/users/{userId:int}")]
    public ActionResult DeleteUser(int userId)
    {   
        User? user = GetUserFromClaims();

        if (user == null)
        {
            return Unauthorized();
        }

        if (user.Type != AccountType.Admin)
        {
            return Forbid();
        }

        User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);

        if (selectedUser == null)
        {
            return NotFound();
        }

        Context.Remove(selectedUser);
        Context.SaveChanges();

        return Ok();
    }

    [HttpGet("name")]
    [Authorize]
    public ActionResult<string> GetNameById(int id)
    {
        return Ok(Context.Users.First(h => h.Id == id).Username);
    }
}

public class UserCreationData
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public AccountType Type { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Unit { get; set; }
}