using Microsoft.Azure.Devices;

namespace IotSmartHome.Function;

public class IoTHubServiceClient
{
    private readonly ServiceClient _client;

    public IoTHubServiceClient()
    {
        var connectionString = Environment.GetEnvironmentVariable("IOTHUB_SERVICE_CONNECTION_STRING");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        _client = ServiceClient.CreateFromConnectionString(connectionString);
    }

    public async Task SendMessageToDeviceAsync(string deviceId, byte[] message)
    {
        using var msg = new Message(message);
        msg.ContentType = "application/json";
        msg.ContentEncoding = "utf-8";
        msg.MessageId = Guid.CreateVersion7().ToString();
        msg.CreationTimeUtc = DateTime.UtcNow;
        await _client.SendAsync(deviceId, msg);
    }
}