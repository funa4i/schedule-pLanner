using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulePlannerBack;
using SchedulePlannerBack.Adapters;
using SchedulePlannerBack.Config;
using SchedulePlannerBack.Domain;
using SchedulePlannerBack.Domain.Infrastructure;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Repository;
using SchedulePlannerBack.Service;
using SchedulePlannerBack.Util;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<EventService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<ParticipantService>();

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<ParticipantRepository>();
builder.Services.AddSingleton<EventRepository>();

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
log.LogInformation("Starting application");
log.LogInformation(builder.Configuration["DataBaseSettings:ConnectionString"]);
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<SchedulePlannerDbContext>();
    var pendingMigrations = dbContext!.Database.GetPendingMigrations();
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();


