using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;
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
            return new OkObjectResult(Context.Users.Where(u => u.UnitId == user.UnitId).ToList());
        }
        return new OkObjectResult(Services.Users.GetAll());
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

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        Services.Users.Edit(userId, data);

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

        Services.Users.Delete(userId);

        return new OkResult();
    }
}