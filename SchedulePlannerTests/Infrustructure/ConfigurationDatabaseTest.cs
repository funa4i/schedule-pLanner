using SchedulePlannerBack.Domain.Infrastructure;

namespace SchedulePlannerTests.Infrustructure;

public class ConfigurationDatabaseTest : IConfigurationDatabase
{
    public string ConnectionString => "Server=localhost;Database=testdb;User Id=sa;Password=password123!;TrustServerCertificate=True;Encrypt=False;";
}