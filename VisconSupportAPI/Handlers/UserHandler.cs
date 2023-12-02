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
    
    public ActionResult<List<Machine>> GetUserMachines(User? user, int userId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.User)
        {
            Context.Entry(user).Collection(u => u.Company.Machines).Load();
            return new OkObjectResult(user.Company.Machines);
        }

        User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);

        if (selectedUser == null)
        {
            return new NotFoundResult();
        }
            
        return new OkObjectResult(Services.Machines.GetAllForUser(selectedUser));
    }
    
    public ActionResult AddUserMachine(User? user, int userId, List<long> machineIds)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        
        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        User? selectedUser = Context.Users.FirstOrDefault(u => u.Id == userId);
        
        if (selectedUser == null)
        {
            return new NotFoundResult();
        }

        if (selectedUser.Type != AccountType.User)
        {
            return new BadRequestResult();
        }
        
        Services.Users.EditMachines(userId, Services.Machines.GetAll().Where(m => machineIds.Contains(m.Id)).ToList());

        return new OkResult();
    }
}