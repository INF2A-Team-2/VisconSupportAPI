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
                    .SetIsOriginAllowed((_) => true)
                    .AllowCredentials();
            });
        });
      
        builder.Services.AddSignalR();
        
        WebApplication app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!Directory.Exists("Logs"))
        {
            Directory.CreateDirectory("Logs");
        }
        
        string path = $"Logs/{DateTime.Now:s}.log.txt";
        
        File.Create(path).Close();

        app.Use((context, next) =>
        {
            LogRequestHeaders(path, context.Request);
            return next();
        });

        app.UseCors("Default");

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<MessageHub>("/messageHub");

        app.Run();
    }
    
    private static void LogRequestHeaders(string path, HttpRequest request)
    {
        File.AppendText(path).WriteLine($"{request.Method} {request.Path}");
        File.AppendText(path).WriteLine("Request Headers:");
        foreach (KeyValuePair<string, StringValues> header in request.Headers)
        {
            File.AppendText(path).WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        File.AppendText(path).WriteLine("\n\n\n");
    }
}
