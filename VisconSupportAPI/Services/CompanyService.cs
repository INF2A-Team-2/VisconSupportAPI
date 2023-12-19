using VisconSupportAPI.Data;
using VisconSupportAPI.Models;
using VisconSupportAPI.Types;

namespace VisconSupportAPI.Services;

public class CompanyService : Service
{
    public CompanyService(DatabaseContext context, IConfiguration configuration, ServicesList services) : base(context, configuration, services)
    {
    }

    public Company? GetById(int id) => Context.Companies.FirstOrDefault(u => u.Id == id);
    
    public List<Company> GetAll() => Context.Companies.ToList();

    public Company Create(NewCompany data)
    {
        Company company = new Company()
        {
            Name = data.Name,
            Latitude = data.Latitude,
            Longitude = data.Longitude
        };
        
        Context.Companies.Add(company);

        Context.SaveChanges();

        return company;
    }

    public void Edit(int id, NewCompany data)
    {
        Company? company = GetById(id);

        if (company == null)
        {
            throw new ArgumentException("User with ID {id} not found", nameof(id));
        }

        company.Name = data.Name;
        company.Latitude = data.Latitude;
        company.Longitude = data.Longitude;

        Context.SaveChanges();
    }

    public void Delete(int id)
    {
        Company? company = GetById(id);

        if (company != null)
        {
            Context.Companies.Remove(company);
            Context.SaveChanges();
        }
    }
}