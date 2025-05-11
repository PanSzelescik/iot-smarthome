namespace IotSmartHome.Extensions;

public static class ConnectionStringExtensions
{
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        ArgumentException.ThrowIfNullOrEmpty(connectionString, name);
        return connectionString;
    }
}