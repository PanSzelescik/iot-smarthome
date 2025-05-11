using System.Collections.Concurrent;

namespace IotSmartHome.Function;

public class IoTHubDeviceClientFactory : IDisposable, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, IoTHubDeviceClient> _clients = new();
    
    public IoTHubDeviceClient GetClient(string device)
    {
        return _clients.GetOrAdd(device, key => new IoTHubDeviceClient(key));
    }
    
    public void Dispose()
    {
        foreach (var client in _clients.Values)
        {
            client.Dispose();
        }
        
        _clients.Clear();
        
        GC.SuppressFinalize(this);
    }
    
    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients.Values)
        {
            await client.DisposeAsync();
        }
        
        _clients.Clear();
        
        GC.SuppressFinalize(this);
    }
}