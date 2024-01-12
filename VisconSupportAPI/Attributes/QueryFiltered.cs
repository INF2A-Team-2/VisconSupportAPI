using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace VisconSupportAPI.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class QueryFiltered : ActionFilterAttribute
{
    public string? DefaultSortKey { get; }
    
    public QueryFiltered(string? defaultSortKey = null)
    {
        this.DefaultSortKey = defaultSortKey;
    }
    
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult { Value: IEnumerable<object> data } res)
        {
            IQueryCollection query = context.HttpContext.Request.Query;
            
            if (query.TryGetValue("sort", out StringValues value) || DefaultSortKey != null)
            {
                string? sort = value == StringValues.Empty ? DefaultSortKey : value;

                if (sort == null)
                {
                    context.Result = new BadRequestResult();
                    base.OnActionExecuted(context);
                    return;
                }

                string[] parts = sort.Split('.');
                string key = parts[0];
                
                if (parts[1] != "asc" && parts[1] != "desc")
                {
                    context.Result = new BadRequestObjectResult("Sort order must be 'asc' (ascending) or 'desc' (descending)");
                    base.OnActionExecuted(context);
                    return;
                }
                
                bool descending = parts[1] == "desc";

                data = SortData(data, key, descending);
            }
            
            if (query.TryGetValue("limit", out value))
            {
                string? limit = value;

                if (limit == null || !int.TryParse(limit, out int limitValue))
                {
                    context.Result = new BadRequestResult();
                    base.OnActionExecuted(context);
                    return;
                }

                data = data.Take(limitValue);
            }
            
            res.Value = data;
        }
        
        base.OnActionExecuted(context);
    }

    private static IEnumerable<object> SortData(IEnumerable<object> data, string key, bool descending) =>
        descending 
            ? data.OrderByDescending(x => x.GetType().GetProperty(Capitalize(key))?.GetValue(x)) 
            : data.OrderBy(x => x.GetType().GetProperty(Capitalize(key))?.GetValue(x));

    private static string Capitalize(string s) => s switch
    {
        "" => "",
        _ => s[0].ToString().ToUpper() + s[1..]
    };
}