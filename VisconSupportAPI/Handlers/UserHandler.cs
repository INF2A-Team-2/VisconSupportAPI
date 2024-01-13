using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class UserHandler : Handler
{
    public UserHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }

    public ActionResult<List<User>> GetAllUsers(User? user)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.Helpdesk)
        {
            return new OkObjectResult(Context.Users.Where(u => u.UnitId == user.UnitId && u.Id != user.Id).ToList());
        }
        return new OkObjectResult(Services.Users.GetAll().Where(u => u.Id != user.Id));
    }
    
    public ActionResult<User> GetUserById(User? user, int userId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        
        User? retrievedUser = Services.Users.GetById(userId);

        return new OkObjectResult(retrievedUser);
    }

    public ActionResult<User> CreateUser(User? user, NewUser data)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        if (data.Username == null || data.Password == null)
        {
            return new BadRequestResult();
        }

        if (Context.Users.Select(u => u.Username).Contains(data.Username))
        {
            return new ConflictResult();
        }

        User createdUser = Services.Users.Create(data);

        Services.Logs.Create(user, $"User: {createdUser.Username} has been added", user: createdUser);    
        
        return new CreatedAtActionResult(
            "GetUser",
            "User",
            new { userId = createdUser.Id },
            createdUser);
    }

    public ActionResult EditUser(User? user, int userId, NewUser data)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin && user.Id != userId)
        {
            return new ForbidResult();
        }

        Services.Users.Edit(userId, data);

        Services.Logs.Create(user, $"User: {data.Username} has been edited"); 

        return new NoContentResult();
    }
    
    public ActionResult DeleteUser(User? user, int userId)
    {   
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        Services.Logs.Create(user, $"User: {user.Username} has been edited", user: user);
        
        Services.Users.Delete(userId);
        
        return new OkResult();
    }
}