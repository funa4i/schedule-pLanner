using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchedulePlannerBack;
using SchedulePlannerBack.Domain;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Domain.Views;
using SchedulePlannerTests.Infrustructure;
using Serilog;

namespace SchedulePlannerTests.Controllers;

public class BaseWebApiControllerTest 
{
    protected static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    protected long userId;
    
    protected static async Task<T?>  GetModelFromResponseAsync<T>(HttpResponseMessage response)
    {
        return JsonSerializer.Deserialize<T?>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);
    }
    protected static StringContent MakeContent(object model) =>
        new(JsonSerializer.Serialize(model), Encoding.UTF8,
            "application/json");
    private WebApplicationFactory<Program> _webApplication;
    protected SchedulePlannerDbContext _context { get; private set; }
    protected HttpClient HttpClient { get; private set; }
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _webApplication = new CustomWebApplicationFactory<Program>();
        HttpClient = _webApplication
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    using var loggerFactory = new LoggerFactory();
                    loggerFactory.AddSerilog(new
                            LoggerConfiguration()
                        .ReadFrom.Configuration(new
                                ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build())
                        .CreateLogger());
                    services.AddSingleton(loggerFactory);
                });
            })
            .CreateClient();
        _context =  _webApplication.Services.GetRequiredService<SchedulePlannerDbContext>();
        _context.RemoveEventsFromDatabase();
        _context.RemoveParticipantFromDatabase();
        _context.RemoveUsersFromDatabase();
        _context.RemoveEventDatesFromDatabase();
        _context.SaveChanges();
        var endpointDataSource = _webApplication.Services.GetRequiredService<EndpointDataSource>();

        var u = new UserRequest("user1", "123");
        HttpClient.PostAsync("/api/Auth/SignUp", MakeContent(u)).GetAwaiter().GetResult();
        var request = HttpClient.PostAsync("/api/Auth/SignIn", MakeContent(u)).Result;
        var data = GetModelFromResponseAsync<JwtResponse>(request).Result;
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {data!.Token}");
        
        
    }
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
        HttpClient?.Dispose();
        _webApplication?.Dispose();
    }
}