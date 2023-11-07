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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    
    [HttpGet("{userId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    [HttpPut("{userId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult PutUser(int userId, UserCreationData data)
    {
        Console.WriteLine("PUTTTTTTTTT");
        Console.WriteLine(data.Username);
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

        if (data.Username != null && Context.Users.Select(u => u.Username)
                .Where(u => u != selectedUser.Username)
                .Contains(data.Username))
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
        
        Context.Entry(selectedUser).Collection(u => u.Machines).Load();

        if (selectedUser.Type != AccountType.User)
        {
            selectedUser.Machines.Clear();
        }

        Context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    
    [HttpGet("{userId:int}/machines")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<Machine>> GetMachines(int userId){
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized("Not authorized");
        }

        if (user.Type == AccountType.User)
        {
            Context.Entry(user).Collection(u => u.Machines).Load();
            return Ok(user.Machines);
        }

        User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);

        if (selectedUser == null)
        {
            return NotFound();
        }
            
        Context.Entry(selectedUser).Collection(u => u.Machines).Load();
        return Ok(selectedUser.Machines);
    }

    [HttpPut("{userId:int}/machines")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult AddMachine(int userId, List<long> machineIds)
    {
        User? user = GetUserFromClaims();
        
        if (user == null)
        {
            return Unauthorized("Not authorized");
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

        if (selectedUser.Type != AccountType.User)
        {
            return BadRequest();
        }
        
        Context.Entry(selectedUser).Collection(u => u.Machines).Load();

        List<Machine> machines = Context.Machines.Where(m => machineIds.Contains(m.Id)).ToList();
        
        selectedUser.Machines.Clear();
        selectedUser.Machines.AddRange(machines);
        
        Context.SaveChanges();

        return Ok();
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