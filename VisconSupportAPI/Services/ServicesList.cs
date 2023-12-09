using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Services;

public class ServicesList
{
    protected readonly DatabaseContext Context;

    protected readonly IConfiguration Configuration;

    public readonly UserService Users;
    public readonly CompanyService Companies;
    public readonly IssueService Issues;
    public readonly MessageService Messages;
    public readonly MachineService Machines;
    public readonly AuthService Auth;
    public readonly AttachmentService Attachments;

    public readonly UnitService Units;
    
    public ServicesList(DatabaseContext context, IConfiguration configuration)
    {
        Context = context;
        Configuration = configuration;

        Users = new UserService(context, configuration, this);
        Companies = new CompanyService(context, configuration, this);
        Issues = new IssueService(context, configuration, this);
        Messages = new MessageService(context, configuration, this);
        Machines = new MachineService(context, configuration, this);
        Auth = new AuthService(context, configuration, this);
        Attachments = new AttachmentService(context, configuration, this);
    }
    
    public IEnumerable<TProperty> LoadCollection<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> selector) 
        where TEntity : class
        where TProperty : class
    {
        CollectionEntry<TEntity, TProperty> collection = Context.Entry(entity).Collection(selector);
        collection.Load();

        return collection.CurrentValue ?? new List<TProperty>();
    }


    public TProperty? LoadReference<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> selector) 
        where TEntity : class
        where TProperty : class 
    {
        ReferenceEntry<TEntity, TProperty> reference = Context.Entry(entity).Reference(selector);
        reference.Load();

        return reference.CurrentValue;
    }
}