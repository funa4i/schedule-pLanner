using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulePlannerBack.Adapters;
using SchedulePlannerBack.Application;
using SchedulePlannerBack.Config;
using SchedulePlannerBack.Domain;
using SchedulePlannerBack.Domain.Infrastructure;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Interfaces.Application;
using SchedulePlannerBack.Repository;
using SchedulePlannerBack.Service;
using SchedulePlannerBack.Util;
using Serilog;

namespace SchedulePlannerBack;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        using var loggerFactory = new LoggerFactory();
        var _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration).CreateLogger();
        loggerFactory.AddSerilog(_logger);
        builder.Services.AddSingleton(loggerFactory.CreateLogger("Any"));
        builder.Services.AddAuthorization();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(builder.Configuration["Jwt:Key"] ?? ""),
                    ValidateIssuerSigningKey = true,
                };
            });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<IEventService, EventService>();
        builder.Services.AddSingleton<JwtService>();
        builder.Services.AddSingleton<IParticipantService, ParticipantService>();

        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IParticipantRepository, ParticipantRepository>();
        builder.Services.AddSingleton<IEventRepository, EventRepository>();

        builder.Services.AddSingleton<SchedulePlannerDbContext>();
        builder.Services.AddSingleton<AuthOptionsConfigurer>();
        builder.Services.AddSingleton<IConfigurationDatabase, ConfigurationDatabase>();
        builder.Services.AddSingleton<AuthenticationAdapter>();
        builder.Services.AddSingleton<EventAdapter>();
        builder.Services.AddSingleton<ParticipantAdapter>();
        builder.Services.AddSingleton<UserAdapter>();

        builder.Services.AddSingleton<UserAuthenticationProvider>();


        var app = builder.Build();
        var log = app.Services.GetRequiredService<ILogger<Program>>();
        log.LogDebug("Starting application");
        log.LogDebug(builder.Configuration["DataBaseSettings:ConnectionString"]);
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<SchedulePlannerDbContext>();
            var pendingMigrations = dbContext!.Database.GetPendingMigrations();
            try
            {
                if (pendingMigrations.Any())
                {
                    log.LogInformation("Applying pending migrations...");
                    dbContext.Database.Migrate();
                    log.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    log.LogInformation("No pending migrations found.");
                }
            }
            catch (Exception e)
            {
                _logger.Warning("Migrations already applied");
            }
            
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.MapControllers();
        app.Run();

    }
}