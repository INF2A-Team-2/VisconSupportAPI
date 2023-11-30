using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Handlers;

public class MachineHandler : Handler
{
    public MachineHandler(ILogger logger, DatabaseContext context, IConfiguration configuration) 
        : base(logger, context, configuration)
    {
    }
    
    public ActionResult<List<Machine>> GetAllMachines(User? user)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type == AccountType.User)
        {
            Services.LoadCollection(user, u => u.Machines);
            return new OkObjectResult(user.Machines);
        }
        
        return new OkObjectResult(Context.Machines);
    }
    
    public ActionResult ImportMachines(User? user, IFormFile formFile)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }

        if (formFile.Length <= 0 || !formFile.FileName.EndsWith(".csv"))
        {
             return new BadRequestResult();
        }

        using (StreamReader reader = new StreamReader(formFile.OpenReadStream())) 
        {
            CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            List<ImportedMachine> readMachines = csv.GetRecords<ImportedMachine>().ToList();

            foreach (ImportedMachine machine in readMachines)
            {
                User? currUser = Services.Users.GetById(machine.UserId);

                if (currUser == null)
                {
                    continue;
                }
                
                Services.LoadCollection(currUser, u => u.Machines);
                
                Machine? exists = Services.Machines.GetByName(machine.Name);
                
                Services.Users.AddMachine(currUser.Id, exists ?? new Machine() { Name = machine.Name });
            }
        }

        return new OkResult();
    }

    public ActionResult AddMachine(User? user, ImportedMachine machine)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (user.Type != AccountType.Admin)
        {
            return new ForbidResult();
        }
        
        User? currUser = Services.Users.GetById(machine.UserId);

        if (currUser == null)
        {
            return new NotFoundResult();
        }
        
        Machine? exists = Services.Machines.GetByName(machine.Name);
        
        Services.Users.AddMachine(currUser.Id, exists ?? new Machine() { Name = machine.Name });
        
        return new OkResult();
    }
}