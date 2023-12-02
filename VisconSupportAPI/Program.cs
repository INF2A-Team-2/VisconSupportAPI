using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VisconSupportAPI.Data;
using VisconSupportAPI.Handlers;
using VisconSupportAPI.Hubs;
using VisconSupportAPI.Models;

namespace VisconSupportAPI;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = """
                              JWT Authorization header using the Bearer scheme.
                              Enter 'Bearer' [space] and then your token in the text input below.
                              Example: 'Bearer 12345abcdef'
                              """,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        });
        
        builder.Services.AddDbContextPool<DatabaseContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString(builder.Configuration["SelectedConnection"])));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        
        builder.Services.AddAuthentication();

        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Default", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials();
            });
        });
      
        builder.Services.AddSignalR();
        
        WebApplication app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.Use((context, next) =>
            {
                LogRequestHeaders(context.Request);
                return next();
            });
        }

        app.UseCors("Default");

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<MessageHub>("/messageHub");

        app.Run();
    }
    
    private static void LogRequestHeaders(HttpRequest request)
    {
        Console.WriteLine($"{request.Method} {request.Path}");
        Console.WriteLine("Request Headers:");
        foreach (KeyValuePair<string, StringValues> header in request.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        Console.WriteLine();
    }
}