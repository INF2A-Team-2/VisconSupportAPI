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

        if (user.Type == AccountType.User)
        {
            Services.LoadCollection(user, u => u.Company.Machines);
            return new OkObjectResult(user.Company.Machines);
        }
        
        return new OkObjectResult(Context.Machines);
    }

    public ActionResult<Machine> GetMachine(User? user, int machineId)
    {
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        Services.LoadCollection(user, u => u.Company.Machines);

        Machine? machine = Services.Machines.GetById(machineId);

        if (machine == null)
        {
            return new NotFoundResult();
        }

        if (user.Type == AccountType.User && !user.Company.Machines.Contains(machine))
        {
            return new ForbidResult();
        }

        return new OkObjectResult(machine);
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
                
                Services.LoadCollection(currUser, u => u.Company.Machines);
                
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
        
        Machine createdMachine = Services.Users.AddMachine(currUser.Id, exists ?? new Machine() { Name = machine.Name });

        Services.Logs.Create(user, "Machine added", user: currUser, machine: createdMachine);
        
        return new OkResult();
    }
}