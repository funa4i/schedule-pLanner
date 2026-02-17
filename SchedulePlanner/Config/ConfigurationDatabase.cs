using SchedulePlannerBack.Domain.Infrastructure;

namespace SchedulePlannerBack.Config;

public class ConfigurationDatabase(IConfiguration configuration) : IConfigurationDatabase
{
    private readonly Lazy<DataBaseSettings> _dataBaseSettings = new(() =>
    {
        var str = configuration["DataBaseSettings:ConnectionString"] 
                  ?? throw new InvalidDataException(nameof(DataBaseSettings));;
        return new DataBaseSettings() { ConnectionString = str };
    });
    public string ConnectionString => _dataBaseSettings.Value.ConnectionString;
}