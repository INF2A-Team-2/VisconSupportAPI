using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Services;
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

        if (user.Type != AccountType.User)
        {
            return new OkObjectResult(Context.Machines);
        }

        Services.LoadReference(user, u => u.Company);

        if (user.Company == null)
        {
            return new OkObjectResult(new List<Machine>());
        }

        Services.LoadCollection(user.Company, c => c.Machines);

        return new OkObjectResult(user.Company.Machines);
    }

    public ActionResult<Machine> GetMachine(User? user, int machineId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }
        
        Machine? machine = Services.Machines.GetById(machineId);

        if (machine == null)
        {
            return new NotFoundResult();
        }

        if (user.Type != AccountType.User)
        {
            return new OkObjectResult(machine);
        }

        Services.LoadReference(user, u=> u.Company);

        if (user.Company == null)
        {
            return new BadRequestResult();
        }

        Services.LoadCollection(user.Company, c => c.Machines);

        if (!user.Company.Machines.Contains(machine))
        {
            return new ForbidResult();
        }

        if (currUser == null)
        {
            return new NotFoundResult();
        }
        
        Machine? exists = Services.Machines.GetByName(machine.Name);
        
        Machine createdMachine = Services.Users.AddMachine(currUser.Id, exists ?? new Machine() { Name = machine.Name });

        Services.Logs.Create(user, "Machine added", user: currUser, machine: createdMachine);
        
        return new OkResult();
    }
        return new OkObjectResult(machine);
    }
    
    // public ActionResult ImportMachines(User? user, IFormFile formFile)
    // {
    //     if (user == null)
    //     {
    //         return new UnauthorizedResult();
    //     }
    //
    //     if (user.Type != AccountType.Admin)
    //     {
    //         return new ForbidResult();
    //     }
    //
    //     if (formFile.Length <= 0 || !formFile.FileName.EndsWith(".csv"))
    //     {
    //          return new BadRequestResult();
    //     }
    //
    //     using (StreamReader reader = new StreamReader(formFile.OpenReadStream())) 
    //     {
    //         CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    //         List<ImportedMachine> readMachines = csv.GetRecords<ImportedMachine>().ToList();
    //
    //         foreach (ImportedMachine machine in readMachines)
    //         {
    //             User? currUser = Services.Users.GetById(machine.UserId);
    //
    //             if (currUser == null)
    //             {
    //                 continue;
    //             }
    //             
    //             Services.LoadCollection(currUser, u => u.Company.Machines);
    //             
    //             Machine? exists = Services.Machines.GetByName(machine.Name);
    //             
    //             Services.Users.AddMachine(currUser.Id, exists ?? new Machine() { Name = machine.Name });
    //         }
    //     }
    //
    //     return new OkResult();
    // }
}