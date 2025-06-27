using System.Text.Json;
using IotSmartHome.Extensions;
using Microsoft.Azure.Devices;

namespace IotSmartHome.Services;

public sealed class IoTHubSenderService(IConfiguration configuration) : IDisposable
{
    private readonly ServiceClient _client = ServiceClient.CreateFromConnectionString(configuration.GetRequiredConnectionString("IoTHubConnectionString"), TransportType.Amqp);

    public async Task SendJsonAsync<T>(string deviceId, T message)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message);
        await SendBytesAsync(deviceId, bytes);
    }

    public async Task SendBytesAsync(string deviceId, byte[] message)
    {
        using var msg = new Message(message);
        msg.MessageId = Guid.CreateVersion7().ToString();
        await _client.SendAsync(deviceId, msg);
    }

    public void Dispose()
    {
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }
}